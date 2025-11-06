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

public enum EnemyAction
{
    Invalid = -2,
    None = -1,
    First,
    Stand = First,
    Move,
    Search, // Enemies do not start off assuming that there is a player to be spotted.
    Chase,
    Attack,
    Turn, // This is a stop and turn animation thing
    Interact,
    Stunned,
    Disable,
    Max
}

public struct EnemyPlayerData
{
    public Player player;
    public int index;
    public bool isInRange;
    public bool isInVision;
    public List<GameObject> eyePointsSeeingPlayer;
    public bool isSpotted;
    public bool isChased;
    // add memorization patterns here?
}

public partial class Enemy : NetworkBehaviour
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

    [Header("Player Tracking")]
    EnemyPlayerData targetPlayerData;
    List<EnemyPlayerData> spottedPlayers;
    private EnemyPlayerData[] players;

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

            InitEnemyPlayerData();
        }
        else
        {
            Debug.LogError("EnemyData is null. Cannot initialize enemy.");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        StopAllCoroutines();
    }

    private void Start()
    {
        StartCoroutine(CheckRangeRoutine());
    }

    public virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        // this is not final setup, just to  remember what goes in which style of update.
        switch (enemyAction)
        {
            case EnemyAction.Stand:
                EnemyStand();
                break;
            case EnemyAction.Move:
                EnemyMove();
                break;
            case EnemyAction.Search:
                EnemySearch();
                break;
            case EnemyAction.Chase:
                EnemyChase();
                break;
            case EnemyAction.Attack:
                EnemyAttack();
                break;
            case EnemyAction.Turn:
                EnemyTurn();
                break;
            case EnemyAction.Interact:
                EnemyInteract();
                break;
            case EnemyAction.Stunned:
                EnemyStunned();
                break;
            case EnemyAction.Disable:
                EnemyDisabled();
                break;
            default:
                Debug.LogError("enemyAction " + enemyAction + " is not accounted for in FixedUpdate.");
                Debug.Break();
                break;
        }
    }

    public virtual void SetEnemyState(EnemyState newState)
    {
        enemyState = newState;
    }

    public virtual void EnemyStand()
    {
        Debug.Log("The enemy is doing nothing.");
    }

    public virtual void EnemyMove()
    {
        Debug.Log("The enemy is moving.");
    }

    public virtual void EnemySearch()
    {
        Debug.Log("The enemy is searching.");
    }

    public virtual void EnemyChase()
    {
        Debug.Log("The enemy is chasing.");
    }

    public virtual void EnemyAttack()
    {
        Debug.Log("The enemy is attacking.");
    }

    public virtual void EnemyTurn()
    {
        Debug.Log("The enemy is turning.");
    }

    public virtual void EnemyInteract()
    {
        Debug.Log("The enemy is interacting.");
    }

    public virtual void EnemyStunned()
    {
        Debug.Log("The enemy is stunned.");
    }

    public virtual void EnemyDisabled()
    {
        Debug.Log("The enemy is disabling.");
    }

    public int GetEnemyPlayerIndexFromPlayer(Player player)
    {
        int index = -1;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].player == player)
            {
                index = i;
                return index;
            }
        }

        Debug.LogError("Could not find EnemyPlayerData for player " + player.playerName);
        Debug.Break();
        return index;
    }
}
