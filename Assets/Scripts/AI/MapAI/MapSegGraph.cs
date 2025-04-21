using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "base_SegGraphData", menuName = "Data/SegGraphData")]
public class MapSegGraph : ScriptableObject
{
    public List<AnimationCurve> curves = new();
    public Dictionary<MapSegmentType, AnimationCurve> curveType;

    public AnimationCurve room;
    public AnimationCurve hallway;
    public AnimationCurve door;
    public AnimationCurve staff;
    public AnimationCurve bathroom;

    private void Awake()
    {
        curves = GetType()
        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(field => field.FieldType == typeof(AnimationCurve))
        .Select(field => (AnimationCurve)field.GetValue(this))
        .ToList();

        foreach (AnimationCurve curve in curves)
        {
            MapSegmentType segType = (MapSegmentType)Enum.Parse(typeof(MapSegmentType), CapitilizeFirst(curve.ToString()), true);

            try
            {
                curveType[segType] = curve;
            }
            catch (KeyNotFoundException)
            {
                curveType.Add(segType, curve);
            }
        }
    }

    private string CapitilizeFirst(String curve)
    {
        if (string.IsNullOrEmpty(curve)) return curve;
        return char.ToUpper(curve[0]) + curve.Substring(1).ToLower();
    }
}
