using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Invalid = -2,
    None = -1,
    First,
    Map = First,
    Spawn,
    Item,
    AI,
    Max
}

public enum MapSegmentType
{
    Invalid = -2,
    None = -1,
    First,
    Entrance = First,
    Room,
    Hallway,
    Door,
    Staff,
    Bathroom,
    Max
}

public partial class MapManager : MonoBehaviour
{
    public List<Node> nodes = new();

    public GameObject mapNode;
    public GameObject spawnNode;
    public GameObject itemNode;
    public GameObject aiNode;

    public Dictionary<NodeType, GameObject> nodeTypeDic = new();

    public NodeData doorNode;
    public NodeData entranceNode;
    public NodeData hallwayNode;
    public NodeData roomNode;
    
    public Dictionary<MapSegmentType, NodeData> mapSegmentNodeDic = new();

    public void AddToNodeList(Node node)
    {
        nodes.Add(node);
    }

    bool PopulateNodeDic()
    {
        if (nodeTypeDic.Count > 0)
        {
            nodeTypeDic.Clear();
        }

        nodeTypeDic.Add(NodeType.First, mapNode);
        nodeTypeDic.Add(NodeType.Spawn, spawnNode);
        nodeTypeDic.Add(NodeType.Item, itemNode);
        nodeTypeDic.Add(NodeType.AI, aiNode);

        if (nodeTypeDic.Count <= 0)
        {
            return false;
        }
        return true;
    }

    bool PopulateMapNodeDic()
    {
        if (mapSegmentNodeDic.Count > 0 )
        {
            mapSegmentNodeDic.Clear();
        }

        mapSegmentNodeDic.Add(MapSegmentType.Door, doorNode);
        mapSegmentNodeDic.Add(MapSegmentType.Entrance, entranceNode);
        mapSegmentNodeDic.Add(MapSegmentType.Hallway, hallwayNode);
        mapSegmentNodeDic.Add(MapSegmentType.Room, roomNode);

        if (mapSegmentNodeDic.Count <= 0 )
        {
            return false;
        }
        return true;
    }
}