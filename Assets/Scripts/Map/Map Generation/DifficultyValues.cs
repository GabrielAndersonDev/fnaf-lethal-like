using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_difficultyValue", menuName = "Map/DifficultyValue")]
public class DifficultyValue : ScriptableObject
{
    public int maxSegmentCount;
    public float hallway;
    public float room;
}
