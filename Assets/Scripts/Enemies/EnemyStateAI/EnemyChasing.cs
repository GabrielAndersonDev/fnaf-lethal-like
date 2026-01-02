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
        Debug.Log("Enemy is now chasing, pre-while statement.");
        Debug.Log("Current state: " + _state + " Alt State: " + State);
        while (true)
        {
            Debug.Log("Enemy is currently chasing, post-while statement.");

            if (_targetPlayerData.eyePointsSeeingPlayer.Count <= 0
                && _targetPlayerData.player != null)
            {
                Debug.Log("Player no longer seen by eyes. Add function for searchin" +
                    "g last known location");
                // this is for when the player just got out of sight, enemy should go to last known location and search around
                StopChasingCoroutines();

                State = EnemyState.Searching;
            }

            if (_targetPlayerData.eyePointsSeeingPlayer.Count >= 1
                && _targetPlayerData.player != null)
            {
                Debug.Log("At least one eye sees the player, continue chasing.");
                ChasingCoroutine ??= StartCoroutine(FollowPlayer(_targetPlayerData));
                LookAtCoroutine ??= StartCoroutine(LookAtObject(_targetPlayerData.player.gameObject, _targetPlayerData.eyePointsSeeingPlayer[0]));
            }

            yield return _waitForSeconds0_2;
        }
    }

    // logic for following player while in chasing state
    private IEnumerator FollowPlayer(EnemyPlayerData player)
    {
        while (true)
        {
            EnemyMoveCoroutine ??= StartCoroutine(EnemyMove());

            float targetPos = Vector3.Distance(transform.position, player.player.transform.position);
            targetPos -= (bodyCollider.radius * 1.25f);

            Vector3 direction = (player.player.transform.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + direction * targetPos;

            Transform playerPos = player.player.transform;
            playerPos.position = targetPosition;

            pathGoal = playerPos;

            yield return _waitForSeconds0_2;
        }
    }

    private IEnumerator TrackPlayerDirection(Player player)
    {
        while (true)
        {
            if (player == null)
            {
                Debug.LogError("Player is null in TrackPlayerDirection.");
                Debug.Break();
                yield break;
            }

            Vector3 lastKnownPos = player.transform.position;
            Vector3 moveDirection = player.MoveDirection;
            // make it so the move direction determines what direction that they're more likely to go.

            yield return _waitForSeconds0_2;
        }
    }

    public void StopChasingCoroutines()
    {
        if (ChasingCoroutine != null)
        {
            StopCoroutine(ChasingCoroutine);
        }

        if (LookAtCoroutine != null)
        {
            StopCoroutine(LookAtCoroutine);
        }

        if (TrackPlayerDirectionCoroutine != null)
        {
            StopCoroutine(TrackPlayerDirectionCoroutine);
        }
    }
}
