using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : ScriptableObject
{
    public MapManager mapManager;
    public MapSegment parentSegment;
    public NodeContainer parentContainer;
    public NodeType nodeType;
}
