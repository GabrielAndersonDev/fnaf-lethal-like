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


        ConnectTwoSegments(entrance, testSegment);
    }

    private void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    private void ConnectTwoSegments(MapSegment initialSegment, MapSegment attachingSegment)
    {
        // I think i'll want to generate connections based on nodes, not segments, but I'll do this for proof of concept
        MapNode initialNode = null;
        MapNode[] initialNodes = initialSegment.GetComponentsInChildren<MapNode>();

        foreach (MapNode mapNode in initialNodes)
        {
            if (mapNode != null && !mapNode.isConnected)
            {
                initialNode = mapNode;
                break;
            }
        }

        MapNode attachingNode = null;
        MapNode[] attachingNodes = attachingSegment.GetComponentsInChildren<MapNode>();

        foreach (MapNode mapNode in attachingNodes)
        {
            if (mapNode != null && !mapNode.isConnected)
            {
                attachingNode = mapNode;
                break;
            }
        }

        if (initialNode == null
            || attachingNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }
        else
        {
            Debug.LogError("Function 'ConnectTwoSegments()' does not work.");
            Debug.Break();
        }
    }
}
