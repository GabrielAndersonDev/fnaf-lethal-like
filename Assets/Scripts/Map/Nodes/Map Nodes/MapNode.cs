using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapNode : Node
{
    public MapSegmentType mapNodeType;
    public bool isConnected;
    public List<MapSegmentType> connectableNodes;

    public void MapNodeInit(MapNodeData data)
    {
        nodeType = NodeType.First;

        if (nodeData != null)
        {
            nodeData = data;

            mapNodeType = data.mapNodeType;
            isConnected = data.isConnected;
            connectableNodes = data.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {nodeData}");
            Debug.Break();
        }
    }
}