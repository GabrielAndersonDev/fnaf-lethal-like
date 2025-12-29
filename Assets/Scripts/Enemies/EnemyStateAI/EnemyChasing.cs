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
                yield break;
            }

            EnemyPlayerData player = _targetPlayerData;

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

                yield return ChasingCoroutine = StartCoroutine(SearchLastKnownLocation());
            }

            if (player.eyePointsSeeingPlayer.Count >= 1)
            {
                yield return ChasingCoroutine ??= StartCoroutine(FollowPlayer(player));
                yield return LookAtCoroutine ??= StartCoroutine(LookAtObject(player.player.gameObject, _targetPlayerData.eyePointsSeeingPlayer[0]));
            }

            yield return _waitForSeconds0_2;
        }
    }

    // logic for following player while in chasing state
    private IEnumerator FollowPlayer(EnemyPlayerData player)
    {
        while (_state == EnemyState.Chasing)
        {
            yield return EnemyMoveCoroutine ??= StartCoroutine(EnemyMove());

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

    // logic for searching last known location of player
    private IEnumerator SearchLastKnownLocation()
    {
        // add logic for going to location and searching based on direction player was running
        while (_state == EnemyState.Chasing)
        {
            yield return _waitForSeconds0_2;
        }
    }

    private IEnumerator TrackPlayerDirection(Player player)
    {
        while (_state == EnemyState.Chasing)
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
