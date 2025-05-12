using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_difficultyValue", menuName = "Map/DifficultyValue")]
public class DifficultyValue : ScriptableObject
{
    // has to have this # min
    public int maxSegmentCount;

    // has to have this #
    public int room;

    // minimum
    public int staff;
    public int bathroom;

    public float diffSegBoost;
}
