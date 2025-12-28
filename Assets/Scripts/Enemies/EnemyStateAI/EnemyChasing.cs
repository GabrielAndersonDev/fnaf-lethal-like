using System.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    private Coroutine ChasingCoroutine;
    private Coroutine LookAtCoroutine;
    private Coroutine TrackPlayerDirectionCoroutine;

    // logic for when enemy is in chasing state
    public IEnumerator EnemyStateChasing()
    {
        while (_state == EnemyState.Chasing)
        {
            if (_targetPlayerData.player == null)
            {
                Debug.LogWarning("targetPlayerData is null in EnemyStateChasing on first run.");
                yield return OnPlayerTargetChange;
            }

            EnemyPlayerData player = players[_targetPlayerData.index];

            if (player.eyePointsSeeingPlayer.Count <= 0)
            {
                Debug.Log("Player no longer seen by eyes. Add function for searchin" +
                    "g last known location");
                // this is for when the player just got out of sight, enemy should go to last known location and search around
                if (ChasingCoroutine != null)
                {
                    StopCoroutine(ChasingCoroutine);
                    StopCoroutine(LookAtCoroutine);
                }

                if (TrackPlayerDirectionCoroutine != null)
                {
                    StopCoroutine(TrackPlayerDirectionCoroutine);
                }

                ChasingCoroutine = StartCoroutine(SearchLastKnownLocation());
            }

            if (player.eyePointsSeeingPlayer.Count >= 1)
            {
                ChasingCoroutine ??= StartCoroutine(FollowPlayer(player));
                LookAtCoroutine ??= StartCoroutine(LookAtObject(player.player.gameObject, _targetPlayerData.eyePointsSeeingPlayer[0]));
            }
        }

        yield return _waitForSeconds0_2;
    }

    // logic for following player while in chasing state
    private IEnumerator FollowPlayer(EnemyPlayerData player)
    {
        while (_state == EnemyState.Chasing)
        {
            EnemyMoveCoroutine ??= StartCoroutine(EnemyMove());

            float targetPos = Vector3.Distance(transform.position, player.player.transform.position);
            targetPos -= (bodyCollider.radius * 1.25f);

            Vector3 direction = (player.player.transform.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + direction * targetPos;

            Transform playerPos = player.player.transform;
            playerPos.position = targetPosition;

            pathGoal = playerPos;
        }

        yield return _waitForSeconds0_2;
    }

    // logic for searching last known location of player
    private IEnumerator SearchLastKnownLocation()
    {
        // add logic for going to location and searching based on direction player was running
        while (_state == EnemyState.Chasing)
        {
            
        }
        yield return _waitForSeconds0_2;
    }

    private IEnumerator TrackPlayerDirection(int playerIndex)
    {
        while (_state == EnemyState.Chasing)
        {
            if (playerIndex <= -1)
            {
                Debug.LogError("PlayerIndex not valid in TrackPlayerDirection.");
                Debug.Break();
                yield break;
            }

            Player player = players[playerIndex].player;

            if (player == null)
            {
                Debug.LogError("Player at playerIndex is null in TrackPlayerDirection.");
                Debug.Break();
                yield break;
            }

            Vector3 lastKnownPos = player.transform.position;
            Vector3 moveDirection = player.MoveDirection;
            // make it so the move direction determines what direction that they're more likely to go.
        }

        yield return _waitForSeconds0_2;
    }

    public void StopChasingCoroutines()
    {
        StopCoroutine(ChasingCoroutine);
        StopCoroutine(LookAtCoroutine);
        StopCoroutine(TrackPlayerDirectionCoroutine);
    }

    public virtual void EnemyStatechase()
    {
        if (_targetPlayerData.player == null)
        {
            Debug.LogWarning("No target player to chase");
            return;
        }

        Debug.Log("Chasing player " + _targetPlayerData.player.name);

        EnemyPlayerData player = players[_targetPlayerData.index];

        if (player.eyePointsSeeingPlayer.Count <= 0)
        {
            Debug.Log("Player no longer seen by eyes. Add function for searching last known location");
            // this is for when the player just got out of sight, enemy should go to last known location and search around
            return;
        }

        // eventually will just be the head looking at them, body will turn separately
        LookAtObject(player.player.gameObject, _targetPlayerData.eyePointsSeeingPlayer[0]);

        if (Vector3.Distance(transform.position, player.player.transform.position) <= attackRange)
        {
            enemyAction = EnemyAction.Attack;
            return;
        }

        Transform goal = player.player.transform;
        pathGoal = goal;
        enemyAction = EnemyAction.Move;
    }
}
