using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    public static MapManager Instance { get; private set; }

    public Dictionary<MapSegmentType, int> segmentCount = new();
    public Dictionary<MapSegment, float> segProb = new();

    GameInfo gameInfo;

    int segMask;

    public MapGraphs graphs;

    public NavMeshSurface navSurface;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitMapMan();
    }

    public void InitMapMan()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            GameManager.Singleton.sessionSeed.Value = UnityEngine.Random.Range(0, 9999999);
        }

        if (GameManager.Singleton != null)
        {
            gameInfo = ScriptableObject.CreateInstance<GameInfo>();

            gameInfo = gameInfo.GetGameInfoFromSerialized(gameInfo, GameManager.Singleton.gameInfo.Value);

            Debug.Log($"Seed is: {gameInfo.Seed}");

            UnityEngine.Random.InitState(gameInfo.Seed);
            PopSegmentDics();
            ItemManager.Singleton.ItemDictionaryInit();
            EnemyManager.Instance.EnemyDicPop();
            segMask = LayerMask.GetMask("MapPrefab");

            LoadMap();

            navSurface.BuildNavMesh();

            // Eventually will add a system for players to pick out their player models, but for now will just use the default one

            PlayerManager.Singleton.SpawnAllPlayers();

        }
        else
        {
            Debug.LogError("MapMan: gameManager is null");
            Debug.Break();
        }
    }

    public void PopSegmentDics()
    {
        segmentCount.Clear();
        HashSet<int> seenValues = new();

        foreach (string stringSeg in Enum.GetNames(typeof(MapSegmentType)))
        {
            MapSegmentType segType = (MapSegmentType)Enum.Parse(typeof(MapSegmentType), stringSeg, true);

            if (segType == MapSegmentType.Invalid ||
                segType == MapSegmentType.Max ||
                segType == MapSegmentType.Door ||
                segType == MapSegmentType.None)
            {
                continue;
            }

            int intVal = (int)segType;
            if (seenValues.Contains(intVal))
            {
                continue;
            }

            segmentCount.Add(segType, 0);
            seenValues.Add(intVal);
        }

        if (segmentCount.Count < 1)
        {
            Debug.LogError("Segment count population error");
            Debug.Break();
        }
    }

    public MapSegment DetermineNextSegment()
    {
        // two condiitons (for now) has available unusedNodes (weighed lower), !isConnected nodes, distance from the entrance (lower # = better rate)

        MapSegment selectedSegment = null;
        float compareF = 0f;

        foreach (MapSegment seg in segProb.Keys)
        {
            if (segProb[seg] > compareF)
            {
                compareF = segProb[seg];
                selectedSegment = seg;
            }
        }

        return selectedSegment;
    }

    public void UpdateSegProb(MapSegment seg)
    {
        float totalFloat = 1f;

        totalFloat *= CalcIsConnected(seg);
        totalFloat *= EntranceDistCalc(seg);

        try
        {
            segProb[seg] = totalFloat;
        }
        catch (KeyNotFoundException)
        {
            segProb.Add(seg, totalFloat);
        }
    }

    public float CalcIsConnected(MapSegment seg)
    {
        float totalFloat = 0f;
        float isNone = 0f;
        int unusableNodes = 0;

        foreach (MapNode node in seg.mapNodes)
        {
            if (node.isConnected || node.isLocked)
            {
                unusableNodes++;
                continue;
            }

            if (node.isNone)
            {
                isNone += 1f;
            }
        }

        if (unusableNodes >= seg.mapNodes.Length)
        {
            totalFloat = 0f;
            seg.checkForGen = true;

            return totalFloat;
        }

        if (!seg.checkForGen)
        {
            totalFloat += 2f;
            return totalFloat;
        }

        totalFloat = graphs.isNoneGraph.Evaluate(isNone /= seg.mapNodes.Length);
        return totalFloat;
    }

    public float EntranceDistCalc(MapSegment seg)
    {
        float totalFloat = graphs.entranceDist.Evaluate(seg.segmentDistance[MapSegmentType.Entrance]);

        if (seg.segmentType == MapSegmentType.Entrance)
        {
            totalFloat = 0f;
        }

        return totalFloat;
    }

    public bool ConnectTwoSegments(MapSegment initSegment, MapNode initNode, MapSegment attSegment, MapNode attNode)
    {
        if (initNode == null
            || attNode == null)
        {
            Debug.LogError("ConnectTwoSegments error: initial or attaching node are null.");
            Debug.Break();
            return false;
        }

        if (RotateSegment(attSegment, attNode, initNode))
        {
            if (!SegmentTransform(attSegment, attNode, initNode))
            {
                MapNode rotateCheck = RotateSegCollideCheck(attSegment, initNode);

                if (rotateCheck == null)
                {

                    if (initNode.connectableNodes.Contains(attSegment.segmentType))
                    {
                        initNode.connectableNodes.Remove(attSegment.segmentType);
                    }

                    segmentCount[attSegment.segmentType]--;
                    attSegment.gameObject.SetActive(false);
                    Destroy(attSegment.gameObject);
                    return false;
                }
                else
                {
                    attNode = rotateCheck;
                }
            }
            attSegment.GetComponent<BoxCollider>().enabled = true;

            initSegment.AddNeighbor(attSegment);
            attSegment.AddNeighbor(initSegment);

            initNode.isConnected = true;

            attNode.isConnected = true;

            if (initSegment.segmentType == MapSegmentType.Hallway)
            {
                foreach (MapNode node in initSegment.mapNodes)
                {
                    if (node.TryGetComponent<HallwayFunction>(out var hallwayFunc))
                    {
                        hallwayFunc.CheckHallwayState();
                    }
                    else
                    {
                        Debug.LogError($"HallwayFunction missing from segment {initSegment.name}");
                        Debug.Break();
                    }
                }
            }

            if (attSegment.segmentType == MapSegmentType.Hallway)
            {
                foreach (MapNode node in attSegment.mapNodes)
                {
                    if (node.TryGetComponent<HallwayFunction>(out var hallwayFunc))
                    {
                        hallwayFunc.CheckHallwayState();
                    }
                    else
                    {
                        Debug.LogError($"HallwayFunction missing from segment {initSegment.name}");
                        Debug.Break();
                    }
                }
            }

            if (attSegment.GetType() == typeof(RoomSegment))
            {
                RoomSegment roomSegment = (RoomSegment)attSegment;

                foreach (EnemySpawnNode node in roomSegment.enemySpawnNodes)
                {
                    node.SetSpawnLocations();
                }
            }

            MapNodeCollisionCheck(attSegment);
            UpdateAllDistances(attSegment);
            return true;
        }
        else
        {
            Debug.LogError("RotateSegment returned false or null");
            Debug.Break();
            return false;
        }
    }

    public bool RotateSegment(MapSegment attSegment, MapNode attNode, MapNode initNode)
    {
        float rotationDelta = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y + 180f);

        attSegment.transform.Rotate(0f, rotationDelta, 0f, Space.World);

        float finalAngleDiff = Mathf.DeltaAngle(attNode.transform.eulerAngles.y, initNode.transform.eulerAngles.y);


        return Mathf.Approximately(Mathf.Abs(finalAngleDiff), 180f);
    }

    public bool SegmentTransform(MapSegment attSegment, MapNode attNode, MapNode initNode)
    {
        Vector3 difference = attSegment.transform.position - attNode.transform.position;
        Vector3 newTransform = difference + initNode.transform.position;

        Vector3 halfExtents = Vector3.Scale(attSegment.GetComponent<BoxCollider>().size * 0.5f, attSegment.transform.lossyScale);

        attSegment.GetComponent<BoxCollider>().enabled = false;

        if (Physics.CheckBox(newTransform, halfExtents, attSegment.transform.rotation, segMask))
        {
            return false;
        }
        else
        {
            attSegment.transform.position = difference + initNode.transform.position;
            return true;
        }
    }

    public MapNode RotateSegCollideCheck(MapSegment attSeg, MapNode initNode)
    {
        foreach (MapNode node in attSeg.mapNodes)
        {
            RotateSegment(attSeg, node, initNode);

            if (SegmentTransform(attSeg, node, initNode))
            {
                return node;
            }
        }

        return null;
    }

    public bool TestSmallest(MapNode initNode)
    {
        MapSegment testSmallest = MapSegmentInit(segmentData.segDataDic[MapSegmentType.Hallway]);
        RotateSegment(testSmallest, testSmallest.mapNodes[0], initNode);

        if (!SegmentTransform(testSmallest, testSmallest.mapNodes[0], initNode))
        {
            testSmallest.gameObject.SetActive(false);
            Destroy(testSmallest.gameObject);
            initNode.isLocked = true;
            return false;
        }

        testSmallest.gameObject.SetActive(false);
        Destroy(testSmallest.gameObject);
        return true;
    }

    public void MapNodeCollisionCheck(MapSegment seg)
    {
        GameObject[] otherMapNodes;

        otherMapNodes = GameObject.FindGameObjectsWithTag("MapNode");

        foreach (MapNode node in seg.mapNodes)
        {
            for (int i = 0;  i < otherMapNodes.Length; i++)
            {
                MapNode otherNode = otherMapNodes[i].GetComponent<MapNode>();
                if (otherNode == node)
                {
                    continue;
                }

                if (otherNode.transform.position == node.transform.position)
                {
                    PostConnectSeg(otherNode, node);
                    UpdateSegProb(otherNode.parentSegment);
                    UpdateSegProb(node.parentSegment);
                }
            }
        }
        
    }

    public void PostConnectSeg(MapNode attNode, MapNode initNode)
    {
        MapSegment attSeg = attNode.parentSegment;
        MapSegment initSeg = initNode.parentSegment;

        if (!attSeg.neighborSegments.Contains(initSeg))
        {
            attSeg.neighborSegments.Add(initSeg);
        }

        if (!initSeg.neighborSegments.Contains(attSeg))
        {
            initSeg.neighborSegments.Add(attSeg);
        }

        attNode.isNone = false;
        attNode.isLocked = false;
        attNode.isConnected = true;

        initNode.isNone = false;
        initNode.isLocked = false;
        initNode.isConnected = true;
    }

    public string CapitalizeFirst(String curve)
    {
        if (string.IsNullOrEmpty(curve)) return curve;
        return char.ToUpper(curve[0]) + curve.Substring(1).ToLower();
    }
}
