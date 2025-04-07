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
        if (!PopulateMapNodeDic())
        {
            Debug.LogError("PopulateMapNodeDic failed.");
            Debug.Break();
            return;
        }

        if (!PopulateNodeDic())
        {
            Debug.LogError("PopulateNodeDic failed.");
            Debug.Break();
            return;
        }

        MapSegment entrance = EntranceGen();
        MapSegment testSegment = MapSegmentInit(mapSegmentData);

        NodeContainer entranceContainer = entrance.nodeContainers[0];
        NodeContainer testContainer = testSegment.nodeContainers[1];

        ConnectTwoSegments(entrance, entranceContainer, testSegment, testContainer);
        
    }

    private void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    private void GiveRandomSegment()
    {

    }

    private void ConnectTwoSegments(MapSegment initialSegment, NodeContainer initContainer, MapSegment attachingSegment, NodeContainer attContainer)
    {
        Node initNode = initContainer.ownedNode;
        Node attNode = attContainer.ownedNode;

        if (initNode == null
            || attNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        if (RotateSegment(attachingSegment, attNode, initNode))
        {
            SegmentTransform(attachingSegment, initialSegment, attNode, initNode);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    private bool RotateSegment(MapSegment segment, Node attNode, Node initNode)
    {
        float angleCorrectNode = attNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1];

        float angleDifference = initNode.transform.eulerAngles[1] - angleCorrectNode;

        segment.transform.rotation = Quaternion.AngleAxis(angleDifference + 180, Vector3.up);

        if (attNode.transform.eulerAngles[1] == initNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1] + 180
            || initNode.transform.eulerAngles[1] == attNode.transform.eulerAngles[1] - segment.transform.eulerAngles[1] - 180)
        {
            return true;
        }

        return false;
    }

    private void SegmentTransform(MapSegment attSegment, MapSegment initSegment, Node attNode, Node initNode)
    {
        Debug.Log(initSegment.transform.position);

        Vector3 initNodeVec = initSegment.transform.position + initNode.transform.position;

        Debug.Log(initNodeVec);

        Vector3 attNodeVec = attSegment.transform.position + attNode.transform.position;

        Debug.Log(attNodeVec);

        Vector3 nodeDistance = initNodeVec - attNodeVec;

        attSegment.transform.Translate(nodeDistance);
    }
}
