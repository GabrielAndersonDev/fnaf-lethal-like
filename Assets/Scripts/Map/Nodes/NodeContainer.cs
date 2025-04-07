using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeContainer : MonoBehaviour
{
    public NodeType nodeType;
    public MapSegmentType mapNodeType;
    public MapSegment parentSegment;
    public MapManager mapManager;

    public NodeData nodeData;
    public GameObject ownedGameObject;
    public Node ownedNode;

    public void InitNodeContainer(MapManager map, MapSegment parentSeg)
    {
        mapManager = map;

        if (mapManager != null)
        {
            parentSegment = parentSeg;

            mapNodeType = parentSeg.segmentType;

            nodeData = mapManager.mapSegmentNodeDic[mapNodeType];
            Debug.Log(nodeData);
        }
        else
        {
            Debug.LogError("mapManager is null");
            Debug.Break();
        }
    }

    public void AttachNodes()
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

                Debug.Log("reaches NodeType.First");

                if (mapNodeType == MapSegmentType.None)
                {
                    Debug.LogError("Map Node has no MapNodeType");
                    Debug.Break();
                    break;
                }

                ownedGameObject = Instantiate(mapManager.nodeTypeDic[NodeType.First].nodePrefab);
                ownedNode = ownedGameObject.GetComponent<Node>();
                ownedNode.SetParentContainer(this);
                ownedNode.SetNodeData(nodeData);

                break;
        }
    }
}
