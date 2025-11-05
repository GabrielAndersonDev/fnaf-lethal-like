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

public enum EnemyState
{
    Invalid = -2,
    None = -1,
    First,
    Default = First,
    StartOfNight,
    PlayerSpotted,
    SoundHeard, // on sound heard, louder sounds take priority over quiet ones (unless source of sound has been spotted after search? - this may only be on harder difficulties)
    Distracted,  //this is for laser pointer on cat or ball on dog, for example
    Disabled,
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

    public EnemyState enemyState;
    public EnemyAction enemyAction;

    [Header("Enemy Spawning")]
    public RoomType spawnRoom;

    [Header("RB")]
    public Rigidbody rb;

    [Header("Player Tracking")]
    EnemyPlayerData targetPlayerData;
    private Dictionary<ulong, EnemyPlayerData> players;
    bool inRange;
    public List<Player> playersInRange;
    Collider[] visionColliders;

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

    public virtual void Update()
    {
        if (canSee 
            && playersInRange.Count > 0
            && inRange == false)
        {
            inRange = true;
            StartCoroutine(CheckLineFieldOfViewRoutine());
        }

        if (!canSee 
            || playersInRange.Count == 0
            && inRange == true)
        {
            inRange = false;
            StopCoroutine(CheckLineFieldOfViewRoutine());
        }
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
}
