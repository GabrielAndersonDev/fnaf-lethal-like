using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Invalid = -2,
    None = -1,
    First,
    Map = First,
    Spawn,
    Item,
    AI,
    Max
}

public enum MapSegmentType
{
    Invalid = -2,
    None = -1,
    First,
    Entrance = First,
    Room,
    Hallway,
    Door,
    Staff,
    Bathroom,
    Max
}

public partial class MapManager : MonoBehaviour
{
    // Temporary for testing map segment generation
    public MapSegmentData mapSegmentData;

    void Start()
    {
        MapSegment entrance = EntranceGen();
        MapSegment testSegment = MapSegmentInit(mapSegmentData);

        Node entranceContainer = entrance.nodes[0];
        Node testContainer = testSegment.nodes[0];

        ConnectTwoSegments(entrance, entranceContainer, testSegment, testContainer);
        
    }

    public void LoadMap()
    {
        Debug.LogError("Function 'LoadMap()' does not work.");
    }

    public void GiveRandomSegment()
    {

    }

    public void ConnectTwoSegments(MapSegment initialSegment, Node initNode, MapSegment attachingSegment, Node attNode)
    {
        Debug.Log(attNode.transform.position);
        Debug.Log(attachingSegment.transform.TransformPoint(attNode.transform.localPosition));

        if (initNode == null
            || attNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        initialSegment.transform.Rotate(0f, 5f, 0f, Space.World);

        if (RotateSegment(attachingSegment, attNode, initialSegment, initNode))
        {
            SegmentTransform(attachingSegment, initialSegment, attNode, initNode);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    public bool RotateSegment(MapSegment attSegment, Node attNode, MapSegment initSegment, Node initNode)
    {
        float rotationDelta = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float finalAngleDiff = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y);


        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, MapSegment initSegment, Node attNode, Node initNode)
    {
        Vector3 difference = attSegment.transform.position - attNode.transform.position;

        attSegment.transform.position = difference + initNode.transform.position;
    }
}
