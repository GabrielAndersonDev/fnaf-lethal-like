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
    public DifficultyValue difficulty;

    public Dictionary<MapSegmentType, int> segmentCount = new();

    void Start()
    {
        PopSegmentCountDic();
        MapSegment entrance = MapSegmentInit(entranceData);
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

    public void TestGen()
    {
        MapSegment entrance = MapSegmentInit(entranceData);

        foreach (MapNode node in entrance.mapNodes) 
        {

        }
    }

    public void ConnectTwoSegments(MapSegment initSegment, Node initNode, MapSegment attSegment, Node attNode)
    {
        if (initNode == null
            || attNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return;
        }

        initSegment.AddNeighbor(attSegment);
        attSegment.AddNeighbor(initSegment);

        if (RotateSegment(attSegment, attNode, initNode))
        {
            SegmentTransform(attSegment, attNode, initNode);
            UpdateAllDistances(attSegment);
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
