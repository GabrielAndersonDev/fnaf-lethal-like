using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public DifficultyValue difficulty;

    public SegmentValueData hallwayValue;
    public SegmentValueData roomValue;

    public Dictionary<MapSegmentType, SegmentValueData> segValueData = new();
    public Dictionary<MapSegmentType, float> baseValues = new();

    public void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");

        MapSegment entrance = MapSegmentInit(entranceData);

        // we need a way to contain floats of every single segType that are used to determine %, add to an array that we randomize the index of to get our answer
        FindPercentage(entrance.mapNodes[0]);

    }

    public void PopSegValue()
    {
        segValueData.Clear();
        segValueData.Add(MapSegmentType.Hallway, hallwayValue);
        segValueData.Add(MapSegmentType.Room, roomValue);
    }

    public void FindPercentage(MapNode node)
    {
        Dictionary<MapSegmentType, float> altValues = baseValues;

        foreach (MapSegmentType mapSeg in altValues.Keys)
        {
            
        }
    }

    public MapSegmentType[] SegmentArrayGen(MapNode node)
    {
        MapSegmentType[] mapSegArray = new MapSegmentType[node.connectableNodes.Length];
        int i = 0;

        foreach (MapSegmentType mapSeg in node.connectableNodes)
        {
            mapSegArray[i++] = mapSeg; 
        }
        return mapSegArray;
    }



    public float TestFloat(MapSegment segment, MapNode node)
    {


        return 0f;
    }
}
