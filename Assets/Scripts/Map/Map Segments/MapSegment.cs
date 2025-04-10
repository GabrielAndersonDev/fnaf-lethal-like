using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapSegment : MonoBehaviour
{
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
}
