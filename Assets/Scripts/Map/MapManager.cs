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
        MapNode testNode = testSegment.mapNodes[1];

        ConnectTwoSegments(entrance, entranceNode, testSegment, testNode);
        
    }

    private void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    private void GiveRandomSegment()
    {

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

        if (RotateSegment(attachingSegment, attachingNode, initialNode))
        {
            SegmentTransform(attachingSegment, initialSegment, attachingNode, initialNode);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    private bool RotateSegment(MapSegment segment, MapNode attachingNode, MapNode initialNode)
    {
        float angleCorrectNode = attachingNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1];

        float angleDifference = initialNode.transform.eulerAngles[1] - angleCorrectNode;

        segment.transform.rotation = Quaternion.AngleAxis(angleDifference + 180, Vector3.up);

        if (initialNode.transform.eulerAngles[1] == attachingNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1] + 180
            || initialNode.transform.eulerAngles[1] == attachingNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1] - 180)
        {
            return true;
        }

        return false;
    }

    private void SegmentTransform(MapSegment attachingSegment, MapSegment initialSegment, MapNode attachingNode, MapNode initialNode)
    {
        Debug.Log(initialSegment.transform.position);
        Debug.Log(attachingNode.transform.position);
        Debug.Log(initialNode.transform.position);

        Vector3 initNode = initialSegment.transform.position + initialNode.transform.position;

        Debug.Log(initNode);

        Vector3 attNode = attachingSegment.transform.position + attachingNode.transform.position;

        Debug.Log(attNode);

        Vector3 nodeDistance = initNode - attNode;

        attachingSegment.transform.Translate(nodeDistance);
    }
}
