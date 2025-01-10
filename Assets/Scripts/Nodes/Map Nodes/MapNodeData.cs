using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeData : ScriptableObject
{
    public MapSegment mapSegment;
    public NodeType isNode;
    public List<NodeType> connectableNodes;
}
