using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapSegment : MonoBehaviour
{
    public Dictionary<MapSegmentType, int> segmentDistance = new();
    public List<MapSegment> neighborSegments = new();

    public MapManager mapManager;
    public MapSegmentData segmentData;
    public GameObject segmentPrefab;
    public int segmentsAllowed;
    public MapSegmentType segmentType;
    public Node[] nodes;

    public void SegmentInit(MapSegmentData data)
    {
        segmentData = data;

        nodes = this.GetComponentsInChildren<Node>();
        
        if (segmentData != null)
        {
            mapManager = data.mapManager;
            segmentPrefab = data.segmentPrefab;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;
        } 
        else
        {
            Debug.LogError($"Segment data init failed, {segmentData}");
            Debug.Break();
        }

        if (nodes != null)
        {
            foreach (Node nodeContainer in nodes)
            {
                nodeContainer.InitNodeContainer(segmentData.mapManager, this);
                //nodeContainer.AttachNodes();
            }

            data.nodes = nodes;
        }
        else
        {
            Debug.LogError($"SegmentInit error: prefabNodes was null :(");
            Debug.Break();
        }
    }

    public void SegDictionaryInit()
    {
        segmentDistance.Clear();

        for (int i = 0; i < (Enum.GetValues(typeof(MapSegmentType)).Length - 4); i++)
        {
            string enumName = Enum.GetName(typeof(MapSegmentType), i);

            segmentDistance.Add((MapSegmentType)Enum.Parse(typeof(MapSegmentType), enumName), 0);
        }
    }

    public void DistanceUpdate(MapSegment newSegment)
    {
        // each segment checks neighboring segments for smallest int in their dictionary and changes it to that +1 unless theirs is the smallest (like in the case of their segment being the MapSegmentType used

        // make something to catch for segmentDistance defaulting to 0!!

        MapSegmentType segType = newSegment.segmentType;

        int newDist = segmentDistance[segType];

        if (segType == this.segmentType)
        {
            newDist = 0;
            this.segmentDistance[segType] = newDist;
            return;
        }

        foreach (MapSegment mapSeg in neighborSegments)
        {
            if (mapSeg.segmentDistance[segType] < newDist)
            {
                newDist = mapSeg.segmentDistance[segType] + 1;
            }
        }


    }

    public void AddNeighbor(MapSegment mapSegment)
    {
        if (mapSegment == null)
        {
            Debug.LogError("AddNeighbor: mapSegment is null");
            Debug.Break();
        }

        neighborSegments.Add(mapSegment);
    }
}
