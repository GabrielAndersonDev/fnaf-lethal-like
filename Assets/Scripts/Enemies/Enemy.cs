using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [Header("Enemy Info")]
    public string enemyName;
    public int enemyID;
    public EnemyData enemyData;
    public Team team;
    public bool isDeactivated;

    [Header("Enemy Spawning")]
    public RoomType spawnRoom;

    [Header("Enemy AI")]
    [SerializeField]
    EnemyAI enemyAI;

    public virtual void InitializeEnemy(EnemyData data)
    {
        enemyData = data;

        if (enemyData != null)
        {
            enemyName = data.enemyName;
            data.enemyID = enemyID;
            team = data.team;
            isDeactivated = data.isDeactivated;
        }
        else
        {
            Debug.LogError("EnemyData is null. Cannot initialize enemy.");
            return;
        }
    }
}
