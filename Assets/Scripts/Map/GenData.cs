using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "base_mapGenData", menuName = "Map/MapGenData")]
public class GenData : ScriptableObject
{
    public DistanceContainer[] distanceContainers;

    void Start()
    {
        distanceContainers = new DistanceContainer[6];

        
    }
}
