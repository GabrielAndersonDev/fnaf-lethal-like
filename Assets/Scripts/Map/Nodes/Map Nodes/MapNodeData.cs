using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeData : NodeData
{
    public MapSegmentType mapNodeType;
    public bool isConnected;
    public List<MapSegmentType> connectableNodes;

    private void Awake()
    {
        nodeType = NodeType.Map;
    }
}