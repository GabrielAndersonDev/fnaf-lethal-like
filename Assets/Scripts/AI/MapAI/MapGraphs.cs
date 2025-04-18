using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_graphs", menuName = "Map/GraphData")]
public class MapGraphs : ScriptableObject
{
    public AnimationCurve entranceDist;
    
    public AnimationCurve isNoneGraph;
}
