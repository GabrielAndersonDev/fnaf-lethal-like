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

public class NodeContainer : MonoBehaviour
{
    public NodeType nodeType;
    public MapNodeType mapNodeType;
    public Node ownedNode;
    public MapManager mapManager;

    void Start()
    {
        switch (nodeType)
        {
            case NodeType.Invalid:
                Debug.LogError("nodeType is Invalid");
                Debug.Break();
                break;
            case NodeType.None:
                Debug.LogError("nodeType is None");
                Debug.Break();
                break;
            case NodeType.First:
                break;
        }
    }
}
