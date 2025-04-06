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

public enum MapNodeType
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

    public Node mapNode;
    public Node spawnNode;
    public Node itemNode;
    public Node aiNode;

    public Dictionary<NodeType, Node> nodeTypeDic = new();

    public NodeData doorNode;
    public NodeData entranceNode;
    public NodeData hallwayNode;
    public NodeData roomNode;
    
    public Dictionary<MapNodeType, NodeData> mapNodeDic = new();

    public void AddToNodeList(Node node)
    {

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
        if (mapNodeDic.Count > 0 )
        {
            mapNodeDic.Clear();
        }

        mapNodeDic.Add(MapNodeType.Door, doorNode);
        mapNodeDic.Add(MapNodeType.Entrance, entranceNode);
        mapNodeDic.Add(MapNodeType.Hallway, hallwayNode);
        mapNodeDic.Add(MapNodeType.Room, roomNode);

        if (mapNodeDic.Count <= 0 )
        {
            return false;
        }
        return true;
    }
}