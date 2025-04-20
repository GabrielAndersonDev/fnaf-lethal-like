using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSegmentData : ScriptableObject
{
    public MapManager mapManager;
    public GameObject segmentPrefab;
    public MapSegmentType segmentType;
    public Node[] nodes;
}
