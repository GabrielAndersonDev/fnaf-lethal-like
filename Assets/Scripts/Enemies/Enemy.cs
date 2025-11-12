using Steamworks;
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
    private int totalPlayers;

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

            if (enemyAIData != null)
            {
                enemyValueDic = new Dictionary<EnemyState, float>(enemyAIData.enemyAIWeight);
            }
            else
            {
                Debug.LogError("EnemyAIData is null in Enemy.");
                Debug.Break();
            }

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
        EnemyStateCheck();
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

    private void InitEnemyPlayerData()
    {
        totalPlayers = PlayerManager.Singleton.players.Count;

        if (totalPlayers <= 0)
        {
            Debug.LogWarning($"No players found when initializing enemy {enemyName} player data.");
            Debug.Break();
        }

        players = new EnemyPlayerData[totalPlayers];
        visionColliders = new Collider[totalPlayers];
        noiseColliders = new Collider[totalPlayers + 15];
        spottedPlayers = new List<EnemyPlayerData>();

        for (int i = 0; i < totalPlayers; i++)
        {
            Player player = PlayerManager.Singleton.players[i];

            if (player == null)
            {
                Debug.LogWarning($"Player reference is null when initializing enemy {enemyName} player data at index {i}.");
                continue;
            }

            EnemyPlayerData playerData = new()
            {
                player = player,
                index = i,
                isInRange = false,
                isInVision = false,
                eyePointsSeeingPlayer = new List<GameObject>(),
                isSpotted = false,
                isChased = false
            };

            players[i] = playerData;
        }

        StartCoroutine(CheckRangeRoutine());
    }

    public virtual void EnemyStateCheck()
    {
        // Implement state checking logic here
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
