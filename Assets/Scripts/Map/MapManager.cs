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

        float attNodeY = attNode.transform.eulerAngles.y + attSegment.transform.eulerAngles.y;
        float initNodeY = initNode.transform.eulerAngles.y + initSegment.transform.eulerAngles.y;

        float rotationDelta = Mathf.DeltaAngle(attNodeY, initNodeY + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float newAttNodeY = attNode.transform.eulerAngles.y + attSegment.transform.eulerAngles.y;
        float newInitNodeY = initNode.transform.eulerAngles.y + initSegment.transform.eulerAngles.y;

        float finalAngleDiff = Mathf.DeltaAngle(newAttNodeY, newInitNodeY);

        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, MapSegment initSegment, NodeContainer attNode, NodeContainer initNode)
    {

        // attachingSegment.transform.TransformPoint(attContainer.transform.localPosition)

        Vector3 attNodeWorld = attNode.transform.position + attSegment.transform.position;
        Vector3 initNodeWorld = initNode.transform.position + initSegment.transform.position;

        Debug.Log(attNodeWorld);
        Debug.Log(initNodeWorld);

        float nodeDist = Vector3.Distance(attNodeWorld, initNodeWorld);

        // has to add the transform of segment to node

        float nodeDistX = attNodeWorld.x - initNodeWorld.x;
        float nodeDistZ = attNodeWorld.z - initNodeWorld.z;

        Vector3 newLocation = new(nodeDistX, 0f, nodeDistZ);

        
    }
}
