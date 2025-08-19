using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnNode : SpawnNode
{
    public EnemySpawnNodeGraph enemySpawnNodeGraph;
    public Dictionary<EnemyType, float> spawnableEnemies = new();
    public bool isSpawned;
    public bool isNone;

    public Vector3 spawnLocation;
    public Quaternion spawnRotation;

    // enough nodes in each area for at least one of every enemy to spawn including special enemies. each Room has a list of enemies to pick from as they spawn they're taken off the list? this prevents dupes and makes it more managable per room
}
