using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_MapSegData", menuName = "Map/MapSegmentData")]
public class MapSegmentData : ScriptableObject
{
    public MapManager mapManager;
    public GameObject segmentPrefab;
    public MapSegmentType segmentType;
    public MapSegGraph mapSegGraph;
    public Node[] nodes;
    public RoomType roomType;

    public List<EnemyType> spawnableEnemies;
}
