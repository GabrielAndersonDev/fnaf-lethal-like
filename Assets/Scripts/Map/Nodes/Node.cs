using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    public NodeType nodeType;
    public MapSegment parentSegment;

    public virtual void InitNode(MapSegment parentSeg)
    {
        parentSegment = parentSeg;
    }
}
