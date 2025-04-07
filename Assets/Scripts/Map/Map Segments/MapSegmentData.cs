using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSegmentData : ScriptableObject
{
    public MapManager mapManager;
    public GameObject segmentPrefab;
    public string segmentName;
    public int segmentsAllowed;
    public MapSegmentType segmentType;
    public NodeContainer[] nodeContainers;
}
