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
        MapSegment entrance = EntranceGen();
        MapSegment testSegment = MapSegmentInit(mapSegmentData);

        Node entranceNode = entrance.mapNodes[0];
        Node testNode = testSegment.mapNodes[0];

        ConnectTwoSegments(entranceNode, testNode);
        
    }

    private void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    private void ConnectTwoSegments(Node initialNode, Node attachingNode)
    {
        if (initialNode == null
            || attachingNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        MapSegment initialSegment = initialNode.GetComponentInParent<MapSegment>();
        MapSegment attachingSegment = attachingNode.GetComponentInParent<MapSegment>();

        if (initialSegment == null
            || attachingSegment == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching MapSegment are null.");
            Debug.Break();
            return;
        }


    }
}
