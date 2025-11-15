using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class Enemy : NetworkBehaviour
{
    [Header("Vision")]
    public bool canSee;
    public List<GameObject> eyePoints = new();
    public float visionRange;
    public float fieldOfView;
    Collider[] visionColliders;

    public virtual void PlayerSpotted(Player player)
    {
        int index = GetEnemyPlayerIndexFromPlayer(player);
        EnemyPlayerData playerData = players[index];
        playerData.isSpotted = true;
        isAwareOfPlayers = true;

        if (!spottedPlayers.Contains(playerData))
        {
            spottedPlayers.Add(playerData);
        }

        players[index] = playerData;
    }

    bool IsPlayerInFront(GameObject eye, Player player)
    {
        if (player == null)
        {
            return false;
        }

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = eye.transform.position.y; // Ignore vertical difference
        Vector3 toPlayer = (playerPosition - eye.transform.position).normalized;
        float dotProduct = Vector3.Dot(eye.transform.forward, toPlayer);

        if (dotProduct > fieldOfView) // Player is in front
        {
            return true;
        }

        return false;
    }

    private IEnumerator CheckRangeRoutine()
    {
        while (canSee)
        {
            bool hasValidPlayers = false;
            CheckVisionRange();

            Dictionary<int, EnemyPlayerData> colliderToPlayerDict = new();

            for (int i = 0; i < visionColliders.Length; i++)
            {
                if (visionColliders[i] == null)
                {
                    break;
                }

                if (visionColliders[i].GetComponentInParent<Player>() != null)
                {
                    Player player = visionColliders[i].GetComponentInParent<Player>();

                    int playerIndex = -1;

                    for (int j = 0; j < players.Length; j++)
                    {
                        if (players[j].player == player)
                        {
                            playerIndex = j;
                            break;
                        }
                    }

                    if (playerIndex <= -1)
                    {
                        continue;
                    }

                    EnemyPlayerData data = players[playerIndex];

                    if (data.player == null)
                    {
                        Debug.LogError("Player reference in EnemyPlayerData is null.");
                        Debug.Break();
                    }

                    hasValidPlayers = true;
                    data.isInRange = true;
                    players[playerIndex] = data;

                    if (!colliderToPlayerDict.ContainsKey(i))
                    {
                        colliderToPlayerDict.Add(i, data);
                    }
                }
                else
                {
                    Debug.LogError("Collider does not have a Player component in its parent.");
                    Debug.Break();
                }
            }
            
            for (int i = 0; i < players.Length; i++)
            {
                if (players.Length == colliderToPlayerDict.Count)
                {
                    hasValidPlayers = true;
                    break;
                }

                if (!colliderToPlayerDict.ContainsValue(players[i]))
                {
                    EnemyPlayerData data = players[i];
                    data.isInRange = false;
                    data.isInVision = false;
                    data.eyePointsSeeingPlayer.Clear();
                    players[i] = data;
                }
            }

            if (!hasValidPlayers)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            colliderToPlayerDict.Clear();

            if (hasValidPlayers)
            {
                hasValidPlayers = false;

                for (int i = 0; i < players.Length; i++)
                {
                    if (!players[i].isInRange)
                    {
                        continue;
                    }

                    EnemyPlayerData data = players[i];

                    if (data.player == null)
                    {
                        Debug.LogError("Player reference in EnemyPlayerData is null.");
                        Debug.Break();
                    }

                    data = CheckFieldOfView(data);

                    if (data.isInVision)
                    {
                        hasValidPlayers = true;
                    }

                    players[i] = data;
                }
            }

            if (!hasValidPlayers)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            if (hasValidPlayers)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].player == null)
                    {
                        Debug.LogError("Player reference in EnemyPlayerData is null.");
                        Debug.Break();
                    }

                    if (!players[i].isInVision
                        || players[i].isSpotted)
                    {
                        continue;
                    }

                    if (players[i].eyePointsSeeingPlayer.Count <= 0)
                    {
                        Debug.LogWarning("No eye points seeing player despite passing FOV check.");
                        continue;
                    }

                    foreach (GameObject eye in players[i].eyePointsSeeingPlayer)
                    {
                        EnemyBatchcastCheck(this, eye, players[i].player);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    // add second co routine for checking line of sight after players are spotted

    private void CheckVisionRange()
    {
        int layerMask = LayerMask.GetMask("Player");
        Physics.OverlapSphereNonAlloc(transform.position, visionRange, visionColliders, layerMask);
    }

    private EnemyPlayerData CheckFieldOfView(EnemyPlayerData data)
    {
        if (data.player == null)
        {
            Debug.LogError("Player reference in EnemyPlayerData is null.");
            Debug.Break();
        }

        if (eyePoints.Count <= 0)
        {
            Debug.LogError("No eye points assigned to enemy " + enemyName);
            Debug.Break();
            return data;
        }

        foreach (GameObject eye in eyePoints)
        {
            if (IsPlayerInFront(eye, data.player))
            {
                if (!data.eyePointsSeeingPlayer.Contains(eye))
                {
                    data.eyePointsSeeingPlayer.Add(eye);
                }
            }
            else
            {
                if (data.eyePointsSeeingPlayer.Contains(eye))
                {
                    data.eyePointsSeeingPlayer.Remove(eye);
                }
            }
        }

        if (data.eyePointsSeeingPlayer.Count > 0)
        {
            data.isInVision = true;
        }
        else
        {
            data.isInVision = false;
        }

        return data;
    }

    public virtual void ProcessRaycastHit(GameObject eye, Player player, NativeArray<RaycastHit> hits)
    {
        int hitCount = 0;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                Player hitPlayer = hit.collider.GetComponentInParent<Player>();

                if (hitPlayer == player)
                {
                    hitCount++;
                }
            }
        }

        if (hitCount >= 3) // At least 3 out of 5 rays hit the player. Will make this more complex in the future, just working with basics for now
        {
            // Player is spotted
            PlayerSpotted(player);
        }
    }
}