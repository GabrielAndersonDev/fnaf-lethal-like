using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_SegmentValueData", menuName = "Map/SegmentValueData")]
public class SegmentValueData : ScriptableObject
{
    public MapSegmentType segmentType;
    public float baseChance;
}
