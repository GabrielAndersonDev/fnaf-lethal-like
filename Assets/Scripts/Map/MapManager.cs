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
    public Dictionary<MapSegmentType, int> segmentCount = new();
    public Dictionary<MapSegment, float> segProb = new();

    public MapGraphs graphs;

    void Start()
    {
        PopSegmentDics();
        PopSegValue();
        PopSegData();

        LoadMap();
    }

    public void PopSegmentDics()
    {
        segmentCount.Clear();
        baseValues.Clear();
        HashSet<int> seenValues = new();

        foreach (string stringSeg in Enum.GetNames(typeof(MapSegmentType)))
        {
            MapSegmentType segType = (MapSegmentType)Enum.Parse(typeof(MapSegmentType), stringSeg, true);

            if (segType == MapSegmentType.Invalid ||
                segType == MapSegmentType.Max ||
                segType == MapSegmentType.Door)
            {
                continue;
            }

            int intVal = (int)segType;
            if (seenValues.Contains(intVal))
            {
                continue;
            }

            if (segType == MapSegmentType.None)
            {
                baseValues.Add(MapSegmentType.None, 0f);
                seenValues.Add(intVal);
                continue;
            }

            if (segType == MapSegmentType.First ||
                segType == MapSegmentType.Entrance)
            {
                segmentCount.Add(segType, 0);
                seenValues.Add(intVal);
                continue;
            }

            segmentCount.Add(segType, 0);
            baseValues.Add(segType, 0f);
            seenValues.Add(intVal);
        }

        if (segmentCount.Count < 1)
        {
            Debug.LogError("Segment count population error");
            Debug.Break();
        }
    }

    public MapSegment DetermineNextSegment()
    {
        // two condiitons (for now) has available unusedNodes (weighed lower), !isConnected nodes, distance from the entrance (lower # = better rate)

        MapSegment selectedSegment = null;

        foreach (MapSegment seg in segments)
        {
            
        }

        return selectedSegment;
    }

    // use this on generation/change to prevent rechecking everything
    public float CalcIsConnected(MapSegment seg)
    {
        // uses nodeCheckCurve based on viability of it? maybe as a tiebreaker for entrance distance. the more available nodes the better

        // isNone doesn't matter in micro, only macro? - no we need it for rolling dif nodes + telling when to stop
        // macro: tiebreaker, higher % OR # of unused nodes? will have to test this in generation to see which i like better. uses isNone and isConnected to see which are useable. each segment when selected should go through and check each node, but on spawn of new segment, those are weighed higher because they are both !isConnected and !isNone. does this make % a better option then?

        // are the values of each possible bool on different curves?

        // weight of !isNone much higher than isNone, so it prioritizes unchecked segments over repeats

        float totalFloat;
        float isNone = 0f;

        if (!seg.checkForGen)
        {
            totalFloat = 2f;
            return totalFloat;
        }

        foreach (MapNode node in seg.mapNodes)
        {
            if (node.isNone)
            {
                isNone += 1f;
            }
        }

        totalFloat = graphs.isNoneGraph.Evaluate(isNone /= seg.mapNodes.Length);

        return totalFloat;
    }

    public float EntranceDistCalc(MapSegment seg)
    {
        float totalFloat = 0f;

        totalFloat = graphs.entranceDist.Evaluate(seg.segmentDistance[MapSegmentType.Entrance]);

        return totalFloat;
    }

    public MapNode SingleSegNodeSearch(MapSegment segment)
    {
        // add something to ignore previously selected nodes aside from isConnected? - using nodes in unusedNodes
        System.Random rnd = new();
        List<MapNode> nodes = new();

        foreach (MapNode node in segment.mapNodes)
        {
            if (!node.isConnected)
            {
                nodes.Add(node);
            }
        }
        int randNode = rnd.Next(nodes.Count);
        return nodes[randNode];
    }

    public void ConnectTwoSegments(MapSegment initSegment, MapNode initNode, MapSegment attSegment, MapNode attNode)
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

            initNode.isConnected = true;
            attNode.isConnected = true;

            UpdateAllDistances(attSegment);
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
        }
    }

    public bool RotateSegment(MapSegment attSegment, MapNode attNode, MapNode initNode)
    {
        float rotationDelta = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float finalAngleDiff = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y);


        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public void SegmentTransform(MapSegment attSegment, MapNode attNode, MapNode initNode)
    {
        Vector3 difference = attSegment.transform.position - attNode.transform.position;

        attSegment.transform.position = difference + initNode.transform.position;
    }
}
