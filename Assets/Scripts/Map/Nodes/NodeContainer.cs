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

public class NodeContainer : MonoBehaviour
{
    public NodeType nodeType;
    public Node ownedNode;

    void Start()
    {
        
    }
}
