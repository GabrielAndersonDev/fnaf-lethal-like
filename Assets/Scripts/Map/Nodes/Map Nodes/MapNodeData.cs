using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeData : NodeData
{
    public MapNodeType mapNodeType;
    public bool isConnected;
    public List<MapNodeType> connectableNodes;
}