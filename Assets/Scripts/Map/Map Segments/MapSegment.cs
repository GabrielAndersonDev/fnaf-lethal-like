using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapSegment : MonoBehaviour
{
    public MapSegmentData segmentData;
    public GameObject segmentPrefab;
    public string segmentName;
    public int segmentsAllowed;
    public MapSegmentType segmentType;
    public NodeContainer[] nodeContainers;

    public void SegmentInit(MapSegmentData data)
    {
        segmentData = data;

        NodeContainer[] prefabContainers = data.segmentPrefab.GetComponentsInChildren<NodeContainer>();

        Debug.Log(prefabContainers.Length);

        if (prefabContainers != null)
        {
            foreach (NodeContainer nodeContainer in prefabContainers)
            {
                nodeContainer.InitNodeContainer(segmentData.mapManager, this);
                nodeContainer.AttachNodes();
            }

            data.nodeContainers = prefabContainers;
        }
        else
        {
            Debug.LogError($"SegmentInit error: prefabNodes was null :(");
            Debug.Break();
        }

        Debug.Log(data.nodeContainers.Length);

        if (segmentData != null)
        {
            segmentPrefab = data.segmentPrefab;
            segmentName = data.segmentName;
            segmentsAllowed = data.segmentsAllowed;
            segmentType = data.segmentType;
            nodeContainers = data.nodeContainers;
        } 
        else
        {
            Debug.LogError($"Segment data init failed, {segmentData}");
            Debug.Break();
        }
    }
}
