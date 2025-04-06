using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeContainer parentContainer;
    public GameObject nodePrefab;
    public NodeType isNode;

    void SetParentContainer(NodeContainer  nodeContainer)
    {
        parentContainer = nodeContainer;

        if (parentContainer == null )
        {
            Debug.LogError("parentContainer is null");
            Debug.Break();
        }
    }

    public void SetNodeData(Node node, NodeData nodeData)
    {
        
    }
}
