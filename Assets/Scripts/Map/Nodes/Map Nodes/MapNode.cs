using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapNode : Node
{
    public MapNodeData data;
    public MapSegmentType mapNodeType;
    public bool isConnected;
    public MapSegmentType[] connectableNodes;

    public override void InitNode(MapManager map, MapSegment parentSeg)
    {
        base.InitNode(map, parentSeg);

        nodeType = NodeType.First;

        if (data != null)
        {
            mapNodeType = data.mapNodeType;
            isConnected = data.isConnected;
            connectableNodes = data.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {data}");
            Debug.Break();
        }
    }
}