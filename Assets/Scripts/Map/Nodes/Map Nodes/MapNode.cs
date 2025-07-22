using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapNode : Node
{
    public MapNodeData nodeData;
    public MapSegmentType mapNodeType;
    public bool isConnected;
    public bool isNone;
    public bool isLocked;
    public List<MapSegmentType> connectableNodes = new();

    public override void InitNode(MapSegment parentSeg)
    {
        base.InitNode(parentSeg);

        nodeData = Instantiate(parentSeg.mapNodeData);

        nodeType = NodeType.First;

        if (nodeData != null)
        {
            mapNodeType = nodeData.mapNodeType;
            isConnected = nodeData.isConnected;
            isNone = nodeData.isNone;
            connectableNodes = nodeData.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {nodeData}");
            Debug.Break();
        }
    }
}