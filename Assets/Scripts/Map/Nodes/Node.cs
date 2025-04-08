using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public MapManager mapManager;
    public MapSegment parentSegment;
    public NodeContainer parentContainer;
    public NodeData nodeData;
    public NodeType nodeType;

    public void InitNode(NodeData data)
    {

        nodeData = data;

        if (data != null)
        {
            mapManager = data.mapManager;
            parentSegment = data.parentSegment;
            parentContainer = data.parentContainer;
            nodeType = data.nodeType;

            Debug.Log("Node data assigned.");
        }
        else
        {
            Debug.LogError($"nodeData is {nodeData}");
            Debug.Break();
        }
    }
}
