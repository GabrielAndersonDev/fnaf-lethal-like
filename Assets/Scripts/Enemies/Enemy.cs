using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum EnemyType
{
    Invalid = -2,
    None = -1,
    First,
    CatEnemy = First,
    DogEnemy,
    Max 
}

public class Enemy : NetworkBehaviour
{
    [Header("Enemy Info")]
    public NetworkVariable<int> enemyID = new();

    public string enemyName;
    public EnemyType enemyType;
    public EnemyData enemyData;
    public Team team;
    public bool isDeactivated;

    [Header("Enemy Spawning")]
    public RoomType spawnRoom;

    [Header("Enemy AI")]
    EnemyAI enemyAI;

    public virtual void InitializeEnemy(EnemyData data)
    {
        enemyData = data;

        if (enemyData != null)
        {
            enemyName = data.enemyName;
            data.enemyID = enemyID.Value;
            team = data.team;
            isDeactivated = data.isDeactivated;
            spawnRoom = data.spawnRoom;
            enemyAI = data.enemyAI;
        }
        else
        {
            Debug.LogError("EnemyData is null. Cannot initialize enemy.");
            return;
        }
    }
}
