using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public DifficultyValue difficulty;

    public SegmentData segmentData;

    public void LoadMap()
    {
        MapSegment entrance = MapSegmentInit(segmentData.segDataDic[MapSegmentType.Entrance]);
        segmentCount[MapSegmentType.Entrance]++;
        segments.Add(entrance);
        UpdateSegProb(entrance);
        GenerateOnSegment(entrance);

        // the max segment count should be variable as well. use rand to get a range between two ints in difficulty? -- inherently variable based on adding hallway end check?
        int runCount = 0;

        while (segments.Count < difficulty.maxSegmentCount)
        {
            MapSegment selectedSeg = DetermineNextSegment();
            UpdateSegProb(selectedSeg);
            //Debug.Log($"Selected seg:{selectedSeg} Run count: {i}");

            Debug.Log($"Sel. seg: {selectedSeg}");

            GenerateOnSegment(selectedSeg);
            runCount++;
            Debug.Log(runCount);
            Debug.Log(segments.Count);
            
            if (runCount > 120)
            {
                Debug.LogWarning("possible infinite loop");
                break;
            }
        }

        

        foreach (MapSegment seg in segments)
        {
            seg.GetComponent<BoxCollider>().enabled = false;
        }
    }

    // add a system for determining which segment to SingleSegNodeSearch from! this means the distance to entrance being lowest while having no previously unsearched nodes, then once all of them are searched, clearing the unusedNode list? then we can go back through. eventually i'll need to add other variables that affect segment spawn chance (distance from entrance = higher likelyhood of office spawn etc)

    // gens segment on to existing one already
    public void GenerateOnSegment(MapSegment seg)
    {
        seg.checkForGen = true;

        foreach (MapNode node in seg.mapNodes)
        {
            if (node.isConnected || node.isLocked || !TestSmallest(node))
            {
                continue;
            }

            Dictionary<MapSegmentType, float> altValues = FindConnectables(node);
            CalculateBaseRates(altValues);
            CalcMandatorySeg(altValues);
            CalcDistScale(seg, altValues);

            MapSegmentType newSegType = RandSegType(altValues);

            if (newSegType == MapSegmentType.None)
            {
                node.isNone = true;
                continue;
            }
            MapSegment newSegment = MapSegmentInit(segmentData.segDataDic[newSegType]);
            if (ConnectTwoSegments(seg, node, newSegment, SingleSegNodeSearch(newSegment)))
            {
                segmentCount[newSegType]++;
                segments.Add(newSegment);
                UpdateSegProb(seg);
                UpdateSegProb(newSegment);
            }
            else
            {
                UpdateSegProb(seg);
            }
        }
    }

    public Dictionary<MapSegmentType, float> FindConnectables(MapNode node)
    {
        Dictionary<MapSegmentType, float> altValues = new();

        foreach (MapSegmentType mapSeg in node.connectableNodes)
        {
            altValues.Add(mapSeg, 1f);
        }

        return altValues;
    }

    // could integrate calculating additional rates into this one in the future. depends on how i figure out how to calc them
    public void CalculateBaseRates(Dictionary<MapSegmentType, float> altValues)
    {
        foreach (MapSegmentType mapSeg in altValues.Keys.ToList())
        {
            altValues[mapSeg] *= segmentData.segValueDic[mapSeg].baseChance;
        }
    }

    public void CalcMandatorySeg(Dictionary<MapSegmentType, float> altValues)
    {
        // use difficulty for now, may change name/purpose etc

        float segCountPercent = segments.Count / difficulty.maxSegmentCount;

        if (segmentCount[MapSegmentType.Room] >= difficulty.room || !altValues.ContainsKey(MapSegmentType.Room))
        {
            if (altValues.ContainsKey(MapSegmentType.Room))
            {
                altValues[MapSegmentType.Room] *= 0f;
            }
        }
        else
        {
            if (segments.Count > 0)
            {
                altValues[MapSegmentType.Room] *= graphs.maxSegCurve.Evaluate(segCountPercent);

                if (segments.Count >= difficulty.maxSegmentCount - 3)
                {
                    foreach (MapSegmentType mapSeg in altValues.Keys.ToList())
                    {
                        if (mapSeg == MapSegmentType.Room)
                        {
                            continue;
                        }

                        altValues[mapSeg] *= 0f;
                    }
                }
            }
        }

        if (segCountPercent >= 0.6f)
        {
            if (segmentCount[MapSegmentType.Staff] < difficulty.staff && altValues.ContainsKey(MapSegmentType.Staff))
            {
                altValues[MapSegmentType.Staff] *= difficulty.diffSegBoost;
            }

            if (segmentCount[MapSegmentType.Bathroom] < difficulty.bathroom && altValues.ContainsKey(MapSegmentType.Bathroom))
            {
                altValues[MapSegmentType.Bathroom] *= difficulty.diffSegBoost;
            }
        }
    }

    public void CalcDistScale(MapSegment seg, Dictionary<MapSegmentType, float> altValues)
    {
        if (seg.mapSegGraph != null)
        {
            foreach (MapSegmentType segType in seg.mapSegGraph.segGraphs.Keys)
            {

                if (!seg.segmentDistance.ContainsKey(segType))
                {
                    // come back and rebalance this to equate for ungenerated segments compared to already generated ones for gen rates
                    continue;
                }

                if (altValues.ContainsKey(segType))
                {
                    float segDist = seg.segmentDistance[segType];

                    altValues[segType] *= seg.mapSegGraph.segGraphs[segType].Evaluate(segDist);
                }
            }
        }
    }

    public MapSegmentType RandSegType(Dictionary<MapSegmentType, float> altValues)
    {
        Dictionary<MapSegmentType, int> segIntPair = new();
        System.Random rand = new();

        int totalInt = 0;
        int selectedInt;

        foreach (MapSegmentType segType in altValues.Keys)
        {
            int segValue = (int)(altValues[segType] * 100);
            segIntPair.Add(segType, segValue);

            totalInt += segValue;
            Debug.Log($"MapSeg: {segType}, AltVal: {altValues[segType]}");
        }

        Debug.Log($"TotalInt: {totalInt}");

        if (totalInt > 0)
        {
            selectedInt = rand.Next(totalInt);
            int compareInt = 0;

            Debug.Break();

            foreach (MapSegmentType segType in segIntPair.Keys)
            {
                compareInt += segIntPair[segType];

                if (compareInt >= selectedInt)
                {
                    return segType;
                }
            }
        } 
        else
        {
            return MapSegmentType.None;
        }
        
        Debug.LogError($"segType not found. {selectedInt}");
        Debug.Break();
        return MapSegmentType.Invalid;
    }

    public void HallwayEndCheck()
    {
        Debug.LogError("HallwayEndCheck does not work yet");
        Debug.Break();
    }
    
    public MapNode SingleSegNodeSearch(MapSegment segment)
    {
        System.Random rnd = new();
        List<MapNode> nodes = new();

        foreach (MapNode node in segment.mapNodes)
        {
            if (!node.isConnected || !node.isLocked)
            {
                nodes.Add(node);
            }
        }
        int randNode = rnd.Next(nodes.Count);
        return nodes[randNode];
    }
}
