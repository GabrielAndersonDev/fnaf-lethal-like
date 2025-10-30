using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

public class MovingEnemy : Enemy
{
    // Used for raycasting for vision
    [SerializeField]
    GameObject faceCube;

    [Header("Enemy Stats")]
    public int baseSpeed;
    public float visionDistance;

    [Header("Enemy Movement AI")]
    [SerializeField]
    EnemyAIData enemyAIData;

    public EnemyState enemyState;
    EnemyAction enemyAction;

    UnityEvent<List<Player>> playerInView;

    Player targetPlayer = null;

    public override void OnNetworkSpawn()
    {
        enemyState = EnemyState.StartOfNight;
        enemyAction = EnemyAction.Stand;

        playerInView = new UnityEvent<List<Player>>();
        playerInView.AddListener(OnPlayerInView);
    }

    private void FixedUpdate()
    {
        // this is not final setup, just to  remember what goes in which style of update.
        switch (enemyAction)
        {
            case EnemyAction.Stand:
                Debug.Log("The enemy is doing nothing.");
                break;
            case EnemyAction.Move:
                Debug.Log("The enemy is moving.");
                EnemyMove();
                break;
            case EnemyAction.Search:
                Debug.Log("The enemy is searching.");
                break;
            case EnemyAction.Chase:
                Debug.Log("The enemy is chasing.");
                break;
            case EnemyAction.Attack:
                Debug.Log("The enemy is attacking.");
                break;
            case EnemyAction.Turn:
                Debug.Log("The enemy is turning.");
                break;
            case EnemyAction.Interact:
                Debug.Log("The enemy is interacting.");
                break;
            case EnemyAction.Stunned:
                Debug.Log("The enemy is stunned.");
                break;
            case EnemyAction.Disable:
                Debug.Log("The enemy is disabling.");
                break;
            default:
                Debug.LogError("enemyAction " +  enemyAction + " is not accounted for in FixedUpdate.");
                Debug.Break();
                break;
        }
    }

    private void Update()
    {
        EnemyStateCheck();
    }

    private void EnemyMove()
    {

    }

    private void EnemyStateCheck()
    {
        List<Player> seenPlayerList = CheckVision();

        if (seenPlayerList.Count >= 1)
        {
            playerInView.Invoke(seenPlayerList);
        }
    }

    private List<Player> CheckVision()
    {
        List<Player> playerList = new();
        playerList.Clear();

        // This will be temporary, may switch to eye points with direction on a switch statement for enemy type. If a prey animatronic can't in the very front of them for a player chase, animation compensates by having head tilt
        Ray ray = new(faceCube.transform.position, faceCube.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, visionDistance))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent(out Player player)
                    && !playerList.Contains(player))
                {
                    playerList.Add(player);
                }
            }
        }

        return playerList;
    }

    void OnPlayerInView(List<Player> playerList)
    {
        // if player is in view, take in factors like how much of player is in view, visibility of area (is it smokey or especially dark?), how close are they to the center of view, to determine if officially spotted. this ALSO should include if this player spotted is closer than, perhaps, a current chase, they will change targets. should compare to chase target. target player bypasses center of view checks and partial view checks until completely out of sight

        bool targetInView = false;

        if (playerList.Contains(targetPlayer))
        {
            targetInView = true;
        }

        foreach (Player player in playerList)
        {
            if (player != targetPlayer)
            {

            }
        }
    }

    private bool PlayerSpotted()
    {
        bool targetSpotted = false;



        return targetSpotted;
    }
}
