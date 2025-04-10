using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public MapSegmentData entranceData;
    public List<MapSegment> segments = new();

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

    public MapSegment MapSegmentInit(MapSegmentData mapSegmentData)
    {

        if (mapSegmentData != null)
        {
            mapSegmentData.mapManager = this;
            GameObject newSegment = Instantiate(mapSegmentData.segmentPrefab, Vector3.zero, Quaternion.identity);

            if (newSegment.TryGetComponent<MapSegment>(out var segmentComponent))
            {
                segmentComponent.SegmentInit(mapSegmentData);

                segments.Add(segmentComponent);

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
}
