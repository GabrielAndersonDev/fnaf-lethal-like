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
    public bool checkForGen;
    public Node[] nodes;
    public MapNode[] mapNodes;
    // will probably make 3 seperate arrays for the different kinds of nodes for accessibility

    public void SegmentDataInit(MapSegmentData data)
    {
        segmentData = data;
        checkForGen = false;

        nodes = this.GetComponentsInChildren<Node>();
        mapNodes = this.GetComponentsInChildren<MapNode>();
        
        if (segmentData != null)
        {
            mapManager = data.mapManager;
            segmentPrefab = data.segmentPrefab;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;

            DistanceInit();
            mapManager.EntranceDistCalc(this);
        } 
        else
        {
            Debug.LogError($"Segment data init failed, {segmentData}");
            Debug.Break();
        }

        if (nodes != null)
        {
            foreach (Node node in nodes)
            {
                node.InitNode(segmentData.mapManager, this);
            };

            data.nodes = nodes;
        }
        else
        {
            Debug.LogError($"SegmentInit error: prefabNodes was null :(");
            Debug.Break();
        }
    }

    public void DistanceInit()
    {
        segmentDistance.Clear();
        segmentDistance.Add(segmentType, 0);

        foreach (MapSegmentType segType in mapManager.segmentCount.Keys)
        {
            if (segType == segmentType)
            {
                continue;
            }

            if (mapManager.segmentCount[segType] > 0)
            {
                int newDist = NeighborSearch(99, segType);

                if (newDist != 99)
                {
                    segmentDistance.Add(segType, newDist);
                }
            }
        }
    }

    public bool DistanceUpdate(MapSegment newSegment)
    {
        MapSegmentType segType = newSegment.segmentType;

        int newDist;

        try
        {
            newDist = segmentDistance[segType];
        }
        catch (KeyNotFoundException)
        {
            newDist = 99;

            newDist = NeighborSearch(newDist, segType);
            segmentDistance.Add(segType, newDist);

            return true;
        }

        newDist = NeighborSearch(newDist, segType);

        if (newDist < segmentDistance[segType])
        {
            segmentDistance[segType] = newDist;
            return true;
        }

        return false;
    }

    public int NeighborSearch(int newDist, MapSegmentType segType)
    {
        foreach (MapSegment neighbor in neighborSegments)
        {
            try
            {
                if (neighbor.segmentDistance[segType] < newDist)
                {
                    newDist = neighbor.segmentDistance[segType];
                }
            }
            catch (KeyNotFoundException)
            {
                
            }
        }

        return newDist + 1;
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

    public void RemoveNeighbor(MapSegment mapSegment)
    {
        if (mapSegment == null)
        {
            Debug.LogError("RemoveNeighbor: mapSeg is null");
            Debug.Break();
        }

        neighborSegments.Remove(mapSegment);
    }
}
