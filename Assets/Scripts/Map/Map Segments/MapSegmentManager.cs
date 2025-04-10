using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public MapSegmentData entranceData;
    public MapSegment EntranceGen()
    {
        if (entranceData != null)
        {
            entranceData.mapManager = this;
            
            GameObject entranceObj = Instantiate(entranceData.segmentPrefab, Vector3.zero, Quaternion.identity);

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
    public MapSegment MapSegmentInit(MapSegmentData mapSegmentData)
    {

        if (mapSegmentData != null)
        {
            mapSegmentData.mapManager = this;
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

    public void NodeToSegmentConnect(MapSegmentData mapSegmentData)
    {
        MapSegment newSegment = MapSegmentInit(mapSegmentData);

        NodeContainer[] newContainers = newSegment.GetComponentsInChildren<NodeContainer>();

        if (newContainers == null)
        {
            Debug.LogError("GenerateSegment error: newNodes is null");
            Debug.Break();
        }

        foreach (NodeContainer nodeContainer in newContainers)
        {
            if (nodeContainer != null)
            {
                
            }
        }
    }
}
