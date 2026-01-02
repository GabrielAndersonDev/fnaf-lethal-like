using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    Attack,
    Turn,
    LookAround,// This is a stop and turn animation thing
    Interact,
    Stunned,
    Deactivated,
    Max
}

[System.Serializable]
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
    public EnemyAIData enemyAIDataRef;
    public Team team;
    public bool isDeactivated;

    [Header("Enemy Spawning")]
    // Change to spawn room list if i decide to make certain animatronics spawn in a variety of rooms 
    public RoomType spawnRoom;
    public GameObject roomSpawnedIn;

    [Header("Body")]
    public Rigidbody rb;
    public CapsuleCollider bodyCollider;
    public NavMeshAgent agent;
    public Transform pathGoal;

    private static WaitForSeconds _waitForSeconds0_2;

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
            
            if (TryGetComponent(out NavMeshAgent navAgent))
            {
                agent = navAgent;
            }
            else
            {
                Debug.LogError("NavMeshAgent is null");
                Debug.Break();
            }

            if (enemyAIDataRef != null)
            {
                enemyAIData = Instantiate(enemyAIDataRef);
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

    private void StartEnemyCoroutines()
    {
        Debug.Log("Before starting coroutines in Enemy base class.");
        StartCoroutine(DetermineStateCoroutine());
        Debug.Log("After starting DetermineStateCoroutine in Enemy base class.");
        StartCoroutine(CheckRangeRoutine());
        StartCoroutine(CheckHearingRoutine());
    }

    private void Start()
    {
        AttackDelay = new WaitForSeconds(attackDelay);
        _waitForSeconds0_2 = new WaitForSeconds(0.2f);
        OnStateChange?.Invoke(EnemyState.None, DefaultState);
        Debug.Log("Current enemy state on spawn: " + State);
        OnStateChange += HandleStateChange;

        StartEnemyCoroutines();
    }

    public virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        // this is not final setup, just to  remember what goes in which style of update.
        //switch (enemyAction)
        //{
        //    case EnemyAction.Stand:
        //        EnemyStand();
        //        break;
        //    case EnemyAction.Move:
        //        EnemyMove();
        //        break;
        //    case EnemyAction.Attack:
        //        EnemyAttack();
        //        break;
        //    case EnemyAction.Turn:
        //        EnemyTurn();
        //        break;
        //    case EnemyAction.LookAround:
        //        EnemyLookAround();
        //        break;
        //    case EnemyAction.Interact:
        //        EnemyInteract();
        //        break;
        //    case EnemyAction.Stunned:
        //        EnemyStunned();
        //        break;
        //    case EnemyAction.Deactivated:
        //        EnemyDeactivated();
        //        break;
        //    default:
        //        Debug.LogError("enemyAction " + enemyAction + " is not accounted for in FixedUpdate.");
        //        Debug.Break();
        //        break;
        //}
    }

    private void InitEnemyPlayerData()
    {
        totalPlayers = PlayerManager.Singleton.players.Count;
        playerToPlayerDataDictionary = new();

        if (totalPlayers <= 0)
        {
            Debug.LogWarning($"No players found when initializing enemy {enemyName} player data.");
            Debug.Break();
        }

        visionColliders = new Collider[totalPlayers];
        noiseColliders = new Collider[totalPlayers + 15];
        spottedPlayers = new List<Player>();

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

            if (!playerToPlayerDataDictionary.ContainsKey(player))
            {
                playerToPlayerDataDictionary.Add(player, playerData);
            }
        }
    }
}
