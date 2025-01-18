using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapManager : MonoBehaviour
{
    private void NodeInit(Node node)
    {

        switch(node.isNode)
        {
            // NodeType.First is the Map node
            case NodeType.First:
                MapNode newMapNode = node.GetComponent<MapNode>();
                newMapNode.MapNodeInit(newMapNode.mapNodeData);
                break;
            case NodeType.Spawn:
                break;
            case NodeType.Item:
                break;
            case NodeType.AI:
                break;
            default:
                Debug.LogError($"NodeInit error: isNode is {node.isNode}.");
                Debug.Break();
                break;
        }
    }
}