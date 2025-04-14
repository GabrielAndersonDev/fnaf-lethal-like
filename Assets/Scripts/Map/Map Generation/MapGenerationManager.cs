using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public DifficultyValue difficulty;

    public SegmentData segmentData;
    public List<MapNode> unusedNodes = new();

    public Dictionary<MapSegmentType, MapSegmentData> segData = new();
    public Dictionary<MapSegmentType, SegmentValueData> segValueData = new();
    public Dictionary<MapSegmentType, float> baseValues = new();

    public void LoadMap()
    {
        MapSegment entrance = MapSegmentInit(segmentData.entrance);

        // we need a way to contain floats of every single segType that are used to determine %, add to an array that we randomize the index of to get our answer
        MapSegment newSeg = GenerateNewSegment(entrance);

        for (int i = 0; i < difficulty.maxSegmentCount; i++)
        {
            newSeg = GenerateNewSegment(newSeg);
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
    }

    public MapSegment GenerateNewSegment(MapSegment seg)
    {
        MapNode initNode = SingleSegNodeSearch(seg);
        Dictionary<MapSegmentType, float> altValues = FindConnectables(initNode);
        CalculateBaseRates(altValues);
        FindPercentage(altValues);

        MapSegmentType newSegType = RandSegType(altValues);

        if (newSegType == MapSegmentType.None)
        {
            unusedNodes.Add(initNode);
            // add a search for new nodes. this needs to account for if other nodes have been set to Unused or not
        }

        MapSegment newSegment = MapSegmentInit(segData[RandSegType(altValues)]);
        ConnectTwoSegments(seg, initNode, newSegment, SingleSegNodeSearch(newSegment));
        return newSegment;
    }

    public Dictionary<MapSegmentType, float> FindConnectables(MapNode node)
    {
        Dictionary<MapSegmentType, float> altValues = baseValues;

        foreach (MapSegmentType mapSeg in node.connectableNodes)
        {
            altValues[mapSeg] = 1f;
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

    public float TestFloat(MapSegment segment, MapNode node)
    {


        return 0f;
    }
}
