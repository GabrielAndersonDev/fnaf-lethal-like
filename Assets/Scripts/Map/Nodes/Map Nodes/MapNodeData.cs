using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeData : ScriptableObject
{
    public NodeType isNode;
    public MapNodeType mapNodeType;
    public bool isConnected;
    public List<MapNodeType> connectableNodes;
}