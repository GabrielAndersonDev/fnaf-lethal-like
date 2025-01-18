using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public MapSegmentData entranceData;
    private MapSegment EntranceGen()
    {
        if (entranceData != null)
        {
            GameObject entranceObj = Instantiate(entranceData.segmentPrefab, transform);

            if (entranceObj.TryGetComponent<MapSegment>(out var entranceComponent))
            {
                entranceComponent.SegmentInit(entranceData);

                return entranceComponent;
            }
        }
        Debug.LogError("EntranceGen error: entranceData is null.");
        return null;
    }
    // Make a function for generating the list too, unless it'll be easier to do it in MapManager
    private MapSegment MapSegmentInit(MapSegmentData mapSegmentData)
    {
        if (mapSegmentData != null)
        {
            // This is a temporary Instatiation. Will eventually need to have the Vector3 and Quaternion be changed based on Node placement and direction
            GameObject newSegment = Instantiate(mapSegmentData.segmentPrefab, Vector3.zero, Quaternion.identity);

            if (newSegment.TryGetComponent<MapSegment>(out var segmentComponent))
            {
                segmentComponent.SegmentInit(mapSegmentData);

                return segmentComponent;
            }
            else
            {
                Debug.LogError("Missing an item");
            }
        }
        else
        {
            Debug.LogError("ItemData missing");
            Debug.Break();
        }

        return null;
    }

    private void NodeToSegmentConnect(MapSegmentData mapSegmentData)
    {
        MapSegment newSegment = MapSegmentInit(mapSegmentData);

        Node[] newNodes = newSegment.GetComponentsInChildren<Node>();

        if (newNodes == null)
        {
            Debug.LogError("GenerateSegment error: newNodes is null");
            Debug.Break();
        }

        foreach (Node node in newNodes)
        {
            if (node != null)
            {
                NodeInit(node);
            }
        }
    }
}
