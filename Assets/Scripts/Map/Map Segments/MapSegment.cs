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
    public string segmentName;
    public int segmentsAllowed;
    public MapSegmentType segmentType;
    public NodeContainer[] nodeContainers;

    public void SegmentInit(MapSegmentData data)
    {
        segmentData = data;

        nodeContainers = this.GetComponentsInChildren<NodeContainer>();
        
        if (segmentData != null)
        {
            mapManager = data.mapManager;
            segmentPrefab = data.segmentPrefab;
            segmentName = data.segmentName;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;
        } 
        else
        {
            Debug.LogError($"Segment data init failed, {segmentData}");
            Debug.Break();
        }

        if (nodeContainers != null)
        {
            foreach (NodeContainer nodeContainer in nodeContainers)
            {
                nodeContainer.InitNodeContainer(segmentData.mapManager, this);
                //nodeContainer.AttachNodes();
            }

            data.nodeContainers = nodeContainers;
        }
        else
        {
            Debug.LogError($"SegmentInit error: prefabNodes was null :(");
            Debug.Break();
        }
    }
}
