using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField]
    NetworkTransform networkTransform;

    [Header("Enemy Info")]
    public NetworkVariable<int> enemyID = new();

    public string enemyName;
    public EnemyType enemyType;
    public EnemyData enemyData;
    public Team team;
    public bool isDeactivated;

    [Header("Enemy Spawning")]
    public RoomType spawnRoom;

    [Header("RB")]
    public Rigidbody rb;

    [Header("Vision")]
    public bool canSee;
    public List<GameObject> eyePoints;
    public float visionRange;
    List<Player> playersInRange = new();

    private Collider[] visionColliders;

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

            visionColliders = new Collider[PlayerManager.Singleton.players.Count];
        }
        else
        {
            Debug.LogError("EnemyData is null. Cannot initialize enemy.");
            return;
        }
    }

    public virtual void Update()
    {
        if (canSee)
        {
            StartCoroutine(CheckRangeRoutine());
        }

        while (playersInRange.Count > 0)
        {
            foreach (GameObject eye in eyePoints)
            {
                foreach (Player player in playersInRange)
                {
                    if (IsPlayerInFront(eye, player))
                    {
                        Ray ray = new Ray(eye.transform.position, (player.transform.position - eye.transform.position).normalized);
                        RaycastHit[] hits = Physics.RaycastAll(ray, visionRange);
                        foreach (RaycastHit hit in hits)
                        {
                            ProcessRaycastHit(eye, hit);
                        }
                    }
                }
            }
        }
    }

    bool IsPlayerInFront(GameObject eye, Player player)
    {
        if (player == null)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.transform.position - eye.transform.position).normalized;
        float angle = Vector3.Angle(eye.transform.forward, directionToPlayer);

        if (Mathf.Abs(angle) > 85
            && Mathf.Abs(angle) < 275) // Player is in front
        {
            return true;
        }

        return false;
    }

    private IEnumerator CheckRangeRoutine()
    {
        while (true)
        {
            CheckVisionRange();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void CheckVisionRange()
    {
        int layerMask = LayerMask.GetMask("Player");
        Physics.OverlapSphereNonAlloc(transform.position, visionRange, visionColliders, layerMask);

        foreach (Collider collider in visionColliders)
        {
            if (collider == null)
            {
                continue;
            }


        }
    }

    public virtual void ProcessRaycastHit(GameObject eye, RaycastHit hit)
    {
        
    }
}
