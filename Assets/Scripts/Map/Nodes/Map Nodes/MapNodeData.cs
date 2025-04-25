using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_NodeData", menuName = "Nodes/Map/NodeData")]
public class MapNodeData : NodeData
{
    public MapSegmentType mapNodeType;
    public bool isConnected;
    public bool isNone;
    public List<MapSegmentType> connectableNodes;
}