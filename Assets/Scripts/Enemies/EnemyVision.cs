using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public partial class Enemy : NetworkBehaviour
{
    [Header("Vision")]
    public bool canSee;
    public List<GameObject> eyePoints = new();
    public float visionRange;
    public float fieldOfView;
    public float lookSpeed;
    [SerializeField]
    Collider[] visionColliders;
    public float finalDotProduct;

    public virtual void PlayerSpotted(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null in PlayerSpotted method.");
            return;
        }

        EnemyPlayerData playerData = playerToPlayerDataDictionary[player];
        playerData.isSpotted = true;
        isAwareOfPlayers = true;

        if (!spottedPlayers.Contains(playerData))
        {
            spottedPlayers.Add(playerData);
        }

        playerToPlayerDataDictionary[player] = playerData;

        if (playerData.player == _targetPlayerData.player)
        {
            _targetPlayerData = playerData;
        }
    }

    bool IsPlayerInFront(GameObject eye, Player player)
    {
        if (player == null)
        {
            return false;
        }

        Vector3 playerPosition = player.transform.position;
        playerPosition.y = eye.transform.position.y; // Ignore vertical difference
        Vector3 toPlayer = playerPosition - eye.transform.position;
        float dotProduct = Vector3.Dot(eye.transform.forward, toPlayer);
        finalDotProduct = dotProduct;

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

            List<Player> validPlayers = new();

            // Connect colliders to players and mark them in range
            for (int i = 0; i < visionColliders.Length; i++)
            {
                if (visionColliders[i] == null)
                {
                    continue;
                }

                if (visionColliders[i].GetComponentInParent<Player>() != null)
                {
                    Player player = visionColliders[i].GetComponentInParent<Player>();

                    EnemyPlayerData data = playerToPlayerDataDictionary[player];

                    if (data.player == null)
                    {
                        Debug.LogError("Player reference in EnemyPlayerData is null.");
                        Debug.Break();
                    }

                    hasValidPlayers = true;
                    data.isInRange = true;
                    validPlayers.Add(player);
                    playerToPlayerDataDictionary[player] = data;

                    visionColliders[i] = null;
                }
                else
                {
                    Debug.LogError("Collider does not have a Player component in its parent.");
                    Debug.Break();
                }
            }

            Dictionary<Player, EnemyPlayerData> tempDictionary = new(playerToPlayerDataDictionary);

            // Mark players not in range with proper flags
            foreach (Player player in tempDictionary.Keys)
            {
                if (!validPlayers.Contains(player))
                {
                    EnemyPlayerData data = playerToPlayerDataDictionary[player];
                    data.isInRange = false;
                    data.isInVision = false;
                    data.eyePointsSeeingPlayer.Clear();
                    playerToPlayerDataDictionary[player] = data;

                    if (data.player == _targetPlayerData.player)
                    {
                        _targetPlayerData = data;
                    }
                }
            }

            if (hasValidPlayers)
            {
                hasValidPlayers = false;

                List<Player> tempValidPlayers = new(validPlayers);

                foreach (Player player in tempValidPlayers)
                {
                    EnemyPlayerData data = playerToPlayerDataDictionary[player];

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
                    else
                    {
                        validPlayers.Remove(player);

                        if (_targetPlayerData.player == data.player)
                        {
                            if (data.isSpotted)
                            {
                                data.isSpotted = false;
                                spottedPlayers.Remove(data);
                            }
                        }
                    }

                    playerToPlayerDataDictionary[player] = data;

                    if (data.player == _targetPlayerData.player)
                    {
                        _targetPlayerData = data;
                    }
                }
            }

            // Perform batchcast checks for players in vision to detect if spotted
            if (hasValidPlayers)
            {
                foreach (Player player in validPlayers)
                {
                    if (player == null)
                    {
                        Debug.LogError("Player reference in EnemyPlayerData is null.");
                        Debug.Break();
                    }

                    EnemyPlayerData data = playerToPlayerDataDictionary[player];

                    if (!data.isInVision)
                    {
                        continue;
                    }

                    if (data.eyePointsSeeingPlayer.Count <= 0)
                    {
                        Debug.LogWarning("No eye points seeing player despite passing FOV check.");
                        continue;
                    }

                    foreach (GameObject eye in data.eyePointsSeeingPlayer)
                    {
                        EnemyBatchcastCheck(this, eye, data.player);
                    }
                }


            }

            yield return _waitForSeconds0_2;
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
        else if (hitCount <= 0)
        {
            EnemyPlayerData data = playerToPlayerDataDictionary[player];

            if (data.isSpotted)
            {
                data.isSpotted = false;
                spottedPlayers.Remove(data);
            }
        }
    }

    public IEnumerator LookAtObject(GameObject obj, GameObject eye)
    {
        float time = 0;
        //var step = lookSpeed * Time.deltaTime;

        Quaternion rot;
        yield return rot = Quaternion.FromToRotation(eye.transform.forward, obj.transform.position - eye.transform.position);
        //Debug.Log(rot);
        float yAxis;
        yield return yAxis = Quaternion.Angle(eye.transform.rotation, rot);
        //Debug.Log(yAxis);
        Quaternion target;
        yield return target = Quaternion.AngleAxis(yAxis, Vector3.up);

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, time);

            time += Time.deltaTime * 2;
            yield return null;
        }
    }
}