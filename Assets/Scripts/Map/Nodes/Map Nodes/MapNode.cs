using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MapNode : Node
{
    public MapNodeType mapNodeType;
    public bool isConnected;
    public List<MapNodeType> connectableNodes;
    public MapSegment parentSegment;

    public MapNodeData mapNodeData;

    public void MapNodeInit(MapNodeData data)
    {
        if (mapNodeData != null)
        {
            mapNodeData = data;

            isNode = data.isNode;
            mapNodeType = data.mapNodeType;
            isConnected = data.isConnected;
            connectableNodes = data.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {mapNodeData}");
            Debug.Break();
        }
    }
}