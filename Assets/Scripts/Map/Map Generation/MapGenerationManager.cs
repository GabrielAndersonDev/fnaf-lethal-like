using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public DifficultyValue difficulty;

    public SegmentData segmentData;

    public Dictionary<MapSegmentType, MapSegmentData> segData = new();
    public Dictionary<MapSegmentType, SegmentValueData> segValueData = new();

    public void LoadMap()
    {
        MapSegment entrance = MapSegmentInit(segmentData.entrance);
        segmentCount[MapSegmentType.Entrance]++;
        segments.Add(entrance);
        UpdateSegProb(entrance);
        GenerateOnSegment(entrance);

        // the max segment count should be variable as well. use rand to get a range between two ints in difficulty?

        for (int i = 0; i < difficulty.maxSegmentCount; i++)
        {
            if (difficulty.maxSegmentCount <= segments.Count)
            {
                Debug.Log("segments count break");
                break;
            }
            MapSegment selectedSeg = DetermineNextSegment();

            GenerateOnSegment(selectedSeg);
            
        }

        foreach (MapSegment seg in segments)
        {
            seg.GetComponent<BoxCollider>().enabled = false;
        }
    }

    // add a system for determining which segment to SingleSegNodeSearch from! this means the distance to entrance being lowest while having no previously unsearched nodes, then once all of them are searched, clearing the unusedNode list? then we can go back through. eventually i'll need to add other variables that affect segment spawn chance (distance from entrance = higher likelyhood of office spawn etc)

    public void PopSegValue()
    {
        // this is temporary for testing until it can be automated
        segValueData.Clear();
        segValueData.Add(MapSegmentType.Entrance, segmentData.entranceValue);
        segValueData.Add(MapSegmentType.Hallway, segmentData.hallwayValue);
        segValueData.Add(MapSegmentType.Room, segmentData.roomValue);
        segValueData.Add(MapSegmentType.Staff, segmentData.staffValue);
        segValueData.Add(MapSegmentType.Bathroom, segmentData.bathroomValue);
        segValueData.Add(MapSegmentType.None, segmentData.none);
    }

    public void PopSegData()
    {
        // temporary for testing as well
        segData.Clear();
        segData.Add(MapSegmentType.Entrance, segmentData.entrance);
        segData.Add(MapSegmentType.Hallway, segmentData.hallway);
        segData.Add(MapSegmentType.Room, segmentData.room);
        segData.Add(MapSegmentType.Bathroom, segmentData.bathroom);
    }

    // gens segment on to existing one already
    public void GenerateOnSegment(MapSegment seg)
    {
        seg.checkForGen = true;

        foreach (MapNode node in seg.mapNodes)
        {
            if (difficulty.maxSegmentCount <= segments.Count)
            {
                return;
            }

            if (node.isConnected || node.isLocked || !TestSmallest(node))
            {
                continue;
            }

            Dictionary<MapSegmentType, float> altValues = FindConnectables(node);
            CalculateBaseRates(altValues);
            CalcDistScale(seg, altValues);
            FindPercentage(altValues);

            MapSegmentType newSegType = RandSegType(altValues);

            if (newSegType == MapSegmentType.None)
            {
                node.isNone = true;
                continue;
            }
            MapSegment newSegment = MapSegmentInit(segData[newSegType]);
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
            altValues[mapSeg] *= segValueData[mapSeg].baseChance;
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

    public void FindPercentage(Dictionary<MapSegmentType, float> altValues)
    {
        float totalFloat = 0f;

        foreach (MapSegmentType segType in altValues.Keys.ToList())
        {
            totalFloat += altValues[segType];
        }

        foreach (MapSegmentType segType in altValues.Keys.ToList())
        {
            altValues[segType] /= totalFloat;
        }
    }

    public MapSegmentType RandSegType(Dictionary<MapSegmentType, float> altValues)
    {
        System.Random rand = new();

        int selectedInt = rand.Next(100);

        int compareInt = 0;

        foreach (MapSegmentType segType in altValues.Keys)
        {
            compareInt += (int)(altValues[segType] * 100);

            if (compareInt >= selectedInt)
            {
                return segType;
            }
        }

        Debug.LogError("segType not found.");
        Debug.Break();
        return MapSegmentType.Invalid;
    }

    public MapNode SingleSegNodeSearch(MapSegment segment)
    {
        // add something to ignore previously selected nodes aside from isConnected? - using nodes in unusedNodes
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
