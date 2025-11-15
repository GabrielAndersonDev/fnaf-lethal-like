using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public List<MapSegment> segments = new();

    public MapSegment MapSegmentInit(MapSegmentData mapSegmentData)
    {
        MapSegmentData segData = Instantiate(mapSegmentData);
        GameObject segPrefab;

        if (segData != null)
        {
            if (segData.segmentPrefab.GetType() == typeof(RoomType))
            {
                segPrefab = segmentData.roomPrefabDic[RoomTypeGet()];
            }
            else
            {
                segPrefab = segData.segmentPrefab;
            }

            GameObject newSegment = Instantiate(segPrefab, worldGeometry.transform);

            if (newSegment.TryGetComponent<MapSegment>(out var segmentComponent))
            {
                segmentComponent.SegmentDataInit(segData);

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
