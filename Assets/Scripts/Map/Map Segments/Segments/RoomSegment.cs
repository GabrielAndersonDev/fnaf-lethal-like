using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomSegment : MapSegment
{
    [Header("Room-Specific Data")]
    public EnemySpawnNode[] enemySpawnNodes;
    public RoomType roomType;
    public bool isEnemyGen;
    public List<EnemyType> spawnableEnemies = new();
    public Dictionary<EnemyType, bool> enemySpawned = new();

    public void Awake()
    {
        enemySpawned.Clear();

        foreach (EnemyType enemyType in spawnableEnemies)
        {
            if (enemySpawned.ContainsKey(enemyType))
            {
                continue;
            }

            enemySpawned.Add(enemyType, false);
        }
    }
}
