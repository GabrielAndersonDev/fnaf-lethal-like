using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class GraphTypeConnect
{
    public MapSegmentType segType;
    public AnimationCurve curve;
}

[CreateAssetMenu(fileName = "base_SegGraphData", menuName = "Map/Graphs/SegGraphData")]
public class MapSegGraph : ScriptableObject
{
    public List<GraphTypeConnect> connects = new();
    public Dictionary<MapSegmentType, AnimationCurve> segGraphs;

    private void OnEnable()
    {
        segGraphs = new Dictionary<MapSegmentType, AnimationCurve>();

        foreach (GraphTypeConnect connect in connects)
        {
            if (!segGraphs.ContainsKey(connect.segType))
            {
                segGraphs.Add(connect.segType, connect.curve);
            }
        }
    }
}
