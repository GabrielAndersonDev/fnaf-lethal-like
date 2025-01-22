using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum SegmentType
{
    Invalid = -2,
    None = -1,
    First,
    MainRoom = First,
    Entrance,
    Hallway,
    Max
}

public class MapSegment : MonoBehaviour
{
    public MapSegmentData segmentData;
    public GameObject segmentPrefab;
    public string segmentName;
    public int segmentsAllowed;
    public SegmentType segmentType;
    public MapNode[] mapNodes;

    public void SegmentInit(MapSegmentData data)
    {
        segmentData = data;

        if (segmentData != null)
        {
            segmentPrefab = data.segmentPrefab;
            segmentName = data.segmentName;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;
            mapNodes = data.mapNodes;
        } 
        else
        {
            Debug.LogError($"Segment data init failed, {segmentData}");
            Debug.Break();
        }
    }
}
