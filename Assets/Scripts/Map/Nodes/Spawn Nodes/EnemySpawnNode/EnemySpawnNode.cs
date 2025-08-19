using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnNode : SpawnNode
{
    public EnemySpawnNodeGraph enemySpawnNodeGraph;
    public bool isSpawned;
    public bool isNone;


    // Location for spawning enemies, will change physical location when final map is made by just adding whatever amount to base transform to get the correct position. either that, or just put the spawn default at the enemies' feet level
    public Vector3 spawnLocation;
    public Quaternion spawnRotation;

    public void SetSpawnLocations()
    {
        spawnLocation = transform.position;
        spawnRotation = transform.rotation;
    }

    // enough nodes in each area for at least one of every enemy to spawn including special enemies. each Room has a list of enemies to pick from as they spawn they're taken off the list? this prevents dupes and makes it more managable per room
}
