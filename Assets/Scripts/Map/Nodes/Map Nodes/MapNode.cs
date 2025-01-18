using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum MapNodeType
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

public class MapNode : Node
{
    public MapNodeType mapNodeType;
    public List<MapNodeType> connectableNodes;

    public Transform nodeOrientation;
    public MapNodeData mapNodeData;

    public void Initialize(MapNodeData data)
    {
        if (mapNodeData != null)
        {
            mapNodeData = data;

            isNode = data.isNode;
            mapNodeType = data.mapNodeType;
            connectableNodes = data.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {mapNodeData}");
            Debug.Break();
        }
    }
}