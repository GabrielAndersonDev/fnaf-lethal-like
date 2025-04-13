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
    public MapNode[] mapNodes;
    // will probably make 3 seperate arrays for the different kinds of nodes for accessibility

    public void SegmentInit(MapSegmentData data)
    {
        segmentData = data;

        nodes = this.GetComponentsInChildren<Node>();
        mapNodes = this.GetComponentsInChildren<MapNode>();
        
        if (segmentData != null)
        {
            mapManager = data.mapManager;
            segmentPrefab = data.segmentPrefab;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;

            DistanceInit();
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

        int newDist = 99;

        foreach (MapSegment neighbor in neighborSegments)
        {
            foreach (MapSegmentType segType in neighbor.segmentDistance.Keys)
            {
                try
                {
                    int segNumTrue = segmentDistance[segType];
                }
                catch (KeyNotFoundException)
                {
                    if (segType != this.segmentType)
                    {
                        newDist = NeighborSearch(newDist, segType);

                        segmentDistance.Add(segType, newDist);
                    }
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
            segmentDistance.Add(segType, newDist + 1);

            return true;
        }

        newDist = NeighborSearch(newDist, segType) + 1;

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

        return newDist;
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
