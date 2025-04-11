using System;
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

    public Dictionary<MapSegmentType, int> segmentCount = new();

    void Start()
    {
        PopSegmentCountDic();
        MapSegment entrance = EntranceGen();
        MapSegment testSegment = MapSegmentInit(mapSegmentData);

        Node entranceContainer = entrance.nodes[0];
        Node testContainer = testSegment.nodes[0];

        ConnectTwoSegments(entrance, entranceContainer, testSegment, testContainer);
        
    }

    public void PopSegmentCountDic()
    {
        segmentCount.Clear();

        for (int i = 0; i < (Enum.GetValues(typeof(MapSegmentType)).Length - 4); i++)
        {
            string enumName = Enum.GetName(typeof(MapSegmentType),i);

            segmentCount.Add((MapSegmentType)Enum.Parse(typeof(MapSegmentType), enumName), 0);
        }

        if (segmentCount.Count < 1)
        {
            Debug.LogError("Segment count population error");
            Debug.Break();
        }
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
        if (initNode == null
            || attNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        initialSegment.AddNeighbor(attachingSegment);
        attachingSegment.AddNeighbor(initialSegment);

        if (RotateSegment(attachingSegment, attNode, initNode))
        {
            SegmentTransform(attachingSegment, attNode, initNode);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    public bool RotateSegment(MapSegment attSegment, Node attNode, Node initNode)
    {
        float rotationDelta = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float finalAngleDiff = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y);


        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, Node attNode, Node initNode)
    {
        Vector3 difference = attSegment.transform.position - attNode.transform.position;

        attSegment.transform.position = difference + initNode.transform.position;
    }
}
