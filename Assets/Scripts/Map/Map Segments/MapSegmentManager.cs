using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    // Make a function for generating the list too, unless it'll be easier to do it in MapManager
    public GameObject MapSegmentInit(MapSegmentData mapSegmentData)
    {
        if (mapSegmentData != null)
        {
            // This is a temporary Instatiation. Will eventually need to have the Vector3 and Quaternion be changed based on Node placement and direction
            GameObject newSegment = Instantiate(mapSegmentData.segmentPrefab, Vector3.zero, Quaternion.identity);

            if (newSegment.TryGetComponent<MapSegment>(out var segmentComponent))
            {
                segmentComponent.SegmentInit(mapSegmentData);

                return newSegment;
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

    public void CreateSegment(MapSegmentData mapSegmentData)
    {

    }
}
