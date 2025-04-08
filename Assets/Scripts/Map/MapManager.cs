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

        if (RotateSegment(attachingSegment, attContainer, initContainer))
        {
            SegmentTransform(attachingSegment, initialSegment, attContainer, initContainer);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    public bool RotateSegment(MapSegment segment, NodeContainer attNode, NodeContainer initNode)
    {
        float attNodeWorldY = transform.TransformPoint(attNode.transform.eulerAngles).y;
        float initNodeWorldY = transform.TransformPoint(initNode.transform.eulerAngles).y;

        Debug.Log(attNodeWorldY);
        Debug.Log(initNodeWorldY);

        float rotationDelta = Mathf.DeltaAngle(initNodeWorldY, attNodeWorldY + 180f);

        segment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float newAttNodeY = transform.TransformPoint(attNode.transform.eulerAngles).y;
        float newInitNodeY = transform.TransformPoint(initNode.transform.eulerAngles).y;

        Debug.Log(newAttNodeY);
        Debug.Log(newInitNodeY);

        float finalAngleDiff = Mathf.DeltaAngle(newAttNodeY, newInitNodeY);

        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, MapSegment initSegment, NodeContainer attNode, NodeContainer initNode)
    {

        // attachingSegment.transform.TransformPoint(attContainer.transform.localPosition)

        Vector3 attNodeWorld = attNode.transform.position;
        Vector3 initNodeWorld = initNode.transform.position;

        Debug.Log(attNodeWorld);
        Debug.Log(initNodeWorld);

        Vector3 nodeDirection = initNodeWorld - attNodeWorld;

        attSegment.transform.Translate(nodeDirection, Space.World);
    }
}
