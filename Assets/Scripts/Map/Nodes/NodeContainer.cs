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
    public Transform transformHelper;

    public NodeData nodeData;
    GameObject ownedGameObject;
    NodeData newNodeData;
    public Node ownedNode;

    public void InitNodeContainer(MapManager map, MapSegment parentSeg)
    {
        mapManager = map;

        if (mapManager != null)
        {
            parentSegment = parentSeg;
            mapNodeType = parentSeg.segmentType;

            nodeData = mapManager.mapSegmentNodeDic[mapNodeType];
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

                ownedGameObject = Instantiate(mapManager.nodeTypeDic[NodeType.First]);

                Debug.Log(Vector3.Distance(this.transform.position, ownedGameObject.transform.position));

                ownedNode = ownedGameObject.GetComponent<Node>();

                AssignNodeData(nodeData);
                
                ownedNode.InitNode(nodeData);

                break;
        }
    }

    public NodeData AssignNodeData(NodeData data)
    {
        newNodeData = data;

        if (newNodeData != null)
        {
            newNodeData.mapManager = mapManager;
            newNodeData.parentSegment = parentSegment;
            newNodeData.parentContainer = this;
            newNodeData.nodeType = nodeType;
            
            return data;
        }
        else
        {
            return null;
        }
    }
}
