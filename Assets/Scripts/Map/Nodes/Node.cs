using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeContainer parentContainer;
    public GameObject nodePrefab;
    public NodeData nodeData;
    public NodeType isNode;

    public void SetParentContainer(NodeContainer nodeContainer)
    {
        parentContainer = nodeContainer;

        if (parentContainer == null )
        {
            Debug.LogError("parentContainer is null");
            Debug.Break();
        }
    }

    public void SetNodeData(NodeData data)
    {
        nodeData = data;

        if (data != null)
        {
            data.parentContainer = parentContainer;
            data.isNode = isNode;

            Debug.Log("Node data assigned.");
        }
        else
        {
            Debug.LogError($"nodeData is {nodeData}");
            Debug.Break();
        }
    }
}
