using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeContainer : MonoBehaviour
{
    public NodeType nodeType;
    public MapNodeType mapNodeType;
    public MapManager mapManager;

    NodeData nodeData;
    GameObject ownedNode;

    void Start()
    {
        switch (nodeType)
        {
            case NodeType.Invalid:

                Debug.LogError("nodeType is Invalid");
                Debug.Break();
                break;

            case NodeType.None:

                Debug.LogError("nodeType is None");
                Debug.Break();
                break;

            case NodeType.First:

                if (mapNodeType == MapNodeType.None)
                {
                    Debug.LogError("Map Node has no MapNodeType");
                    Debug.Break();
                    break;
                }

                nodeData = mapManager.mapNodeDic[mapNodeType];

                ownedNode = Instantiate(mapManager.nodeTypeDic[NodeType.First].nodePrefab);
                
                break;
        }
    }
}
