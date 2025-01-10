using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum NodeType
{
    Invalid = -2,
    None = -1,
    First,
    Entrance = First,
    Room,
    Hallway,
    Door,
    PlayerSpawn,
    Max
}

public class MapNode : MonoBehaviour
{
    public MapNodeData mapNodeData;
    public MapSegment mapSegment;
    public NodeType isNode;
    public List<NodeType> connectableNodes;

    public void Initialize(MapNodeData data)
    {
        mapNodeData = data;

        if (mapNodeData.GetComponent<MapNodeData>())
        {
            mapSegment = data.mapSegment;
            isNode = data.isNode;
            connectableNodes = data.connectableNodes;
        }
        else
        {
            Debug.LogError($"mapNodeData is {mapNodeData}");
            Debug.Break();
        }
    }
}
