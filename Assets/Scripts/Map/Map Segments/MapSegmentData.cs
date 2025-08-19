using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_MapSegData", menuName = "Map/MapSegmentData")]
public class MapSegmentData : ScriptableObject
{
    public GameObject segmentPrefab;
    public MapSegmentType segmentType;
}
