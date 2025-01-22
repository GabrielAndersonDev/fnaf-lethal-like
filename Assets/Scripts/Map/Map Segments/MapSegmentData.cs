using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSegmentData : ScriptableObject
{
    public GameObject segmentPrefab;
    public string segmentName;
    public int segmentsAllowed;
    public SegmentType segmentType;
    public MapNode[] mapNodes;

    private void Awake()
    {
        MapNode[] prefabNodes = segmentPrefab.GetComponentsInChildren<MapNode>();

        if (prefabNodes != null)
        {
            mapNodes = prefabNodes;
        }
        else
        {
            Debug.LogError("MapSegmentData error: Error with setting up prefabNodes");
        }
        
    }
}
