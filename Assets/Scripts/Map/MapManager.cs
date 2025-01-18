using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{    
    // Temporary for testing map segment generation
    public MapSegmentData mapSegmentData;

    void Start()
    {
        // Call an initialization that gets the map segment working, then assigns nodes their properties no matter node type
        GenerateSegment(mapSegmentData);
    }

    private void GenerateSegment(MapSegmentData mapSegmentData)
    {
        GameObject newSegment = MapSegmentInit(mapSegmentData);

        Node[] newNodes = newSegment.GetComponentsInChildren<Node>();

        foreach (Node node in newNodes)
        {
            if (node != null)
            {
                NodeInit(node);
            }
        }
    }

    void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    void ConnectTwoSegments(MapSegment segmentOne, MapSegment segmentTwo)
    {

    }
}
