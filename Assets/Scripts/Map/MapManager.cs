using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapSegmentManager mapSegmentManager;
    MapNodeManager mapNodeManager;

    // Temporary for testing map segment generation
    public MapSegmentData mapSegmentData;
    
    void Start()
    {
        mapSegmentManager = GetComponentInChildren<MapSegmentManager>();
        mapNodeManager = GetComponentInChildren<MapNodeManager>();
        mapSegmentManager.MapSegmentInit(mapSegmentData);
    }

    void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    void ConnectTwoSegments(MapSegment segmentOne, MapSegment segmentTwo)
    {

    }
}
