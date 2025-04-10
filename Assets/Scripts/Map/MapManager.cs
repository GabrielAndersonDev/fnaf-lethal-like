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
        NodeContainer testContainer = testSegment.nodeContainers[0];

        ConnectTwoSegments(entrance, entranceContainer, testSegment, testContainer);
        
    }

    public void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    public void GiveRandomSegment()
    {

    }

    public void ConnectTwoSegments(MapSegment initialSegment, NodeContainer initContainer, MapSegment attachingSegment, NodeContainer attContainer)
    {
        Debug.Log(attContainer.transform.position);
        Debug.Log(attachingSegment.transform.TransformPoint(attContainer.transform.localPosition));

        if (initContainer == null
            || attContainer == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        initialSegment.transform.Rotate(0f, 5f, 0f, Space.World);

        if (RotateSegment(attachingSegment, attContainer, initialSegment, initContainer))
        {
            SegmentTransform(attachingSegment, initialSegment, attContainer, initContainer);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    public bool RotateSegment(MapSegment attSegment, NodeContainer attNode, MapSegment initSegment, NodeContainer initNode)
    {
        float rotationDelta = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float finalAngleDiff = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y);


        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, MapSegment initSegment, NodeContainer attNode, NodeContainer initNode)
    {
        Vector3 difference = attSegment.transform.position - attNode.transform.position;

        attSegment.transform.position = difference + initNode.transform.position;
    }
}
