using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    public MapSegmentData entranceData;
    public List<MapSegment> segments = new();

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
                segmentCount[segmentComponent.segmentType] += 1;

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
