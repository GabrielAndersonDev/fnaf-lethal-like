using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    public NodeType nodeType;
    public MapSegment parentSegment;
    public MapManager mapManager;

    public virtual void InitNode(MapManager map, MapSegment parentSeg)
    {
        mapManager = map;

        if (mapManager != null)
        {
            parentSegment = parentSeg;
        }
        else
        {
            Debug.LogError("mapManager is null");
            Debug.Break();
        }
    }
}
