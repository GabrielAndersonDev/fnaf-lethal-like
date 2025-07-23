using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public List<MapSegment> segments = new();

    public MapSegment MapSegmentInit(MapSegmentData mapSegmentData)
    {
        GameObject segPrefab;

        if (mapSegmentData != null)
        {
            if (mapSegmentData.roomType != RoomType.None)
            {
                segPrefab = segmentData.roomPrefabDic[RoomTypeGet()];
            }
            else
            {
                segPrefab = mapSegmentData.segmentPrefab;
            }

            mapSegmentData.mapManager = this;
            GameObject newSegment = Instantiate(segPrefab, Vector3.zero, Quaternion.identity);

            if (newSegment.TryGetComponent<MapSegment>(out var segmentComponent))
            {
                segmentComponent.SegmentDataInit(mapSegmentData);

                return segmentComponent;
            }
            else
            {
                Debug.LogError("MapSegment is missing from newSegment");
                Debug.Break();
            }
        }
        else
        {
            Debug.LogError("MapSegmentData missing");
            Debug.Break();
        }

        return null;
    }

    public void UpdateAllDistances(MapSegment mapSegment)
    {
        mapSegment.DistanceUpdate(mapSegment);

        UpdateDistanceLoop(mapSegment);
    }

    public void UpdateDistanceLoop(MapSegment mapSegment)
    {
        foreach (MapSegment neighbor in mapSegment.neighborSegments)
        {
            if (neighbor.DistanceUpdate(mapSegment))
            {
                UpdateDistanceLoop(neighbor);
            }
        }
    }
}
