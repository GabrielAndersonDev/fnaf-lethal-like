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

        MapNode entranceNode = entrance.mapNodes[0];
        MapNode testNode = testSegment.mapNodes[0];

        ConnectTwoSegments(entrance, entranceNode, testSegment, testNode);
        
    }

    private void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    private void ConnectTwoSegments(MapSegment initialSegment, MapNode initialNode, MapSegment attachingSegment, MapNode attachingNode)
    {
        if (initialNode == null
            || attachingNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        float angleDifference = initialNode.transform.rotation[2] - attachingNode.transform.rotation[2];

        attachingSegment.transform.rotation = Quaternion.AngleAxis(angleDifference, Vector3.forward);

        Debug.Log(angleDifference);

        Vector3 transformDifference = attachingNode.transform.position - initialNode.transform.position;

        attachingSegment.transform.Translate(attachingSegment.transform.position - transformDifference);

        Debug.Log(initialNode.transform.rotation.eulerAngles);
    }
}
