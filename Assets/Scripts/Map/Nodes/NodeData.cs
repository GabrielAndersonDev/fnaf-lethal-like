using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : ScriptableObject
{
    public NodeType nodeType;
    public MapSegment parentSegment;
    public MapManager mapManager;
}
