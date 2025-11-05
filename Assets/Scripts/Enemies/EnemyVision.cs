using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct EnemyEyeData
{
    public GameObject eyePoint;
    public List<Player> playersInVision;
}

public partial class Enemy : NetworkBehaviour
{
    [Header("Vision")]
    public bool canSee;
    public List<EnemyEyeData> eyePointData = new();
    public float visionRange;
    public float fieldOfView;

    public float dotProduct;

    private void InitEnemyPlayerData()
    {
        players = new Dictionary<ulong, EnemyPlayerData>();
        playersInRange = new List<Player>();
        visionColliders = new Collider[PlayerManager.Singleton.players.Count];

        foreach (PlayerProfileData profileData in NetworkScript.Singleton.allPlayerProfileData)
        {
            ulong steamID = profileData.steamID;

            Player player = PlayerManager.Singleton.GetPlayerBySteamID(steamID);

            EnemyPlayerData playerData = new()
            {
                player = player,
                isSpotted = false,
                isChased = false
            };

            if (players.ContainsKey(steamID))
            {
                Debug.LogWarning($"Enemy {enemyName} already contains player data for player {player.playerName}.");
                continue;
            }

            players.Add(steamID, playerData);
        }
    }

    public virtual void PlayerSpotted(Player player)
    {
        Debug.Log($"Enemy {enemyName} has spotted player {player.playerName}!");

        EnemyPlayerData playerData = players[player.steamID];
        playerData.isSpotted = true;
        players[player.steamID] = playerData;
    }

    bool IsPlayerInFront(EnemyEyeData eye, Player player)
    {
        if (player == null)
        {
            return false;
        }

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = eye.eyePoint.transform.position.y; // Ignore vertical difference
        Vector3 toPlayer = (playerPosition - eye.eyePoint.transform.position).normalized;
        dotProduct = Vector3.Dot(eye.eyePoint.transform.forward, toPlayer);

        if (dotProduct > fieldOfView) // Player is in front
        {
            return true;
        }

        return false;
    }

    void SendBatchcast(EnemyEyeData eyePointData, Player player)
    {
        if (eyePointData.eyePoint == null)
        {
            Debug.LogError("Eye point is null.");
            Debug.Break();
        }

        if (eyePointData.playersInVision.Count == 0)
        {
            Debug.Log("No players in vision.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player is null in SendBatchcast.");
            Debug.Break();
        }

        EnemyManager.Singleton.enemyRaycast.EnemyBatchcastCheck(this, eyePointData.eyePoint, player);
    }

    private IEnumerator CheckRangeRoutine()
    {
        while (true)
        {
            CheckVisionRange();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator CheckLineFieldOfViewRoutine()
    {
        while (true)
        {
            CheckAllForFieldOfView();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // add second co routine for checking line of sight after players are spotted

    private void CheckAllForFieldOfView()
    {
        foreach (Player player in playersInRange)
        {
            foreach (EnemyEyeData eyeData in eyePointData)
            {
                if (IsPlayerInFront(eyeData, player)
                    && !eyeData.playersInVision.Contains(player))
                {
                    eyeData.playersInVision.Add(player);
                }

                if (!IsPlayerInFront(eyeData, player)
                    && eyeData.playersInVision.Contains(player))
                {
                    eyeData.playersInVision.Remove(player);
                }

                if (eyeData.playersInVision.Contains(player))
                {
                    SendBatchcast(eyeData, player);
                }
            }
        }
    }

    private void CheckVisionRange()
    {
        int layerMask = LayerMask.GetMask("Player");
        Physics.OverlapSphereNonAlloc(transform.position, visionRange, visionColliders, layerMask);

        List<ulong> playersInRangeKeys = new();

        foreach (Collider collider in visionColliders)
        {
            if (collider == null)
            {
                continue;
            }

            Player player = collider.GetComponentInParent<Player>();

            if (player != null)
            {
                playersInRangeKeys.Add(player.steamID);
            }
            else
            {
                Debug.LogError("Collider with Player LayerMask does not have a Player component.");
                Debug.Break();
            }
        }

        foreach (ulong steamID in players.Keys)
        {
            EnemyPlayerData playerData = players[steamID];

            if (playersInRangeKeys.Contains(steamID))
            {
                if (!playersInRange.Contains(playerData.player))
                {
                    playersInRange.Add(playerData.player);
                }
            }
            else
            {
                if (playersInRange.Contains(playerData.player))
                {
                    playersInRange.Remove(playerData.player);
                }
            }
        }

        for (int i = 0; i < visionColliders.Length; i++)
        {
            visionColliders[i] = null;
        }
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

        Debug.Log(hitCount + " rays hit player " + player.playerName);

        if (hitCount >= 3) // At least 3 out of 5 rays hit the player. Will make this more complex in the future, just working with basics for now
        {
            // Player is spotted
            PlayerSpotted(player);
        }
    }

    public void SetCanSee(bool value)
    {
        canSee = value;

        if (canSee)
        {
            StartCoroutine(CheckRangeRoutine());
        }
        else
        {
            StopCoroutine(CheckRangeRoutine());
        }
    }
}