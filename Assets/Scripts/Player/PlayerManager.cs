using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public enum PlayerPrefabType
{
    Invalid = -2,
    None = -1,
    First,
    Basic = First,
    Alt,
    Max
}

[System.Serializable]
public class PlayerTypePrefabObj
{
    public PlayerPrefabType type;
    public GameObject prefab;
    public PlayerData data;
}

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Singleton {  get; private set; }

    public PlayerSpawnNode spawnNode;

    [SerializeField]
    List<PlayerTypePrefabObj> playerTypePrefabObjList = new();

    public Dictionary<PlayerPrefabType, GameObject> playerTypePrefabDic;
    public Dictionary<PlayerPrefabType, PlayerData> playerTypeDataDic;

    private void Awake()
    {
        if (Singleton != null
            && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Singleton = this;
        }

        InitPlayerPrefabDic();
    }

    private void InitPlayerPrefabDic()
    {
        playerTypePrefabDic = new();
        playerTypeDataDic = new();

        playerTypePrefabDic.Clear();
        playerTypeDataDic.Clear();

        foreach (PlayerTypePrefabObj obj in playerTypePrefabObjList)
        {
            if (obj.type == PlayerPrefabType.Invalid
                || obj.type == PlayerPrefabType.None
                || obj.type == PlayerPrefabType.Max)
            {
                Debug.LogWarning("Skipping Invalid, None, or Max player type: " + obj.type);
                continue;
            }

            if (!playerTypePrefabDic.ContainsKey(obj.type))
            {
                playerTypePrefabDic.Add(obj.type, obj.prefab);
            }
            else
            {
                Debug.LogWarning("PlayerTypePrefabDic contains key " + obj.type + " already.");
            }

            if (!playerTypeDataDic.ContainsKey(obj.type))
            {
                playerTypeDataDic.Add(obj.type, obj.data);
            }
            else
            {
                Debug.LogWarning("PlayerTypeDataDic contains key " + obj.type + " already.");
            }
        }
    }

    public void SpawnAllPlayers()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.GetComponent<NetworkObject>().Despawn();
            }

            PlayerProfileData? profile;

            if (client.ClientId == NetworkManager.Singleton.LocalClientId)
            {
                profile = NetworkScript.Singleton.localPlayerProfileData;
            }
            else
            {
                profile = NetworkScript.Singleton.steamIdToProfileDataDic.ContainsKey(client.ClientId) ?
                    NetworkScript.Singleton.steamIdToProfileDataDic[client.ClientId] : null;
            }

            if (profile != null)
            {
                PlayerPrefabType prefabType = profile.Value.playerPrefabType;

                if (prefabType == PlayerPrefabType.Invalid
                    || prefabType == PlayerPrefabType.None
                    || prefabType == PlayerPrefabType.Max
                    || !playerTypePrefabDic.ContainsKey(prefabType))
                {
                    prefabType = PlayerPrefabType.Basic;
                    Debug.LogWarning("No profile data for client " + client.ClientId + ", using Basic player type.");
                }

                GameObject playerPrefab = playerTypePrefabDic[prefabType];
                Vector3 spawnPos = spawnNode.GiveSpawnLocation(playerPrefab);

                playerPrefab = Instantiate(playerPrefab, spawnPos, playerPrefab.transform.rotation);

                if (playerPrefab.TryGetComponent<NetworkObject>(out var player))
                {
                    player.SpawnAsPlayerObject(client.ClientId, true);
                }
            }
            else
            {
                // Test out adding call to client to request profile data again?
                Debug.LogError("No profile data for client " + client.ClientId + ", using Basic player type.");
                Debug.Break();
            }
        }
    }

    // Player spawn should be controlled by NetworkManager, the RPC is to init the data on clients

    public void SpawnPlayer(ulong player, bool isNewSpawn, Vector3? oldPosition, Quaternion? oldRotation, RpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(player))
        {
            Debug.LogWarning("Player spawn error: Client is not connected.");
            return;
        }

        if (NetworkManager.Singleton.ConnectedClients[player].PlayerObject != null)
        {
            NetworkManager.Singleton.ConnectedClients[player].PlayerObject.GetComponent<NetworkObject>().Despawn();
        }

        ulong steamId = NetworkScript.Singleton.clientIdToSteamId[player];
        PlayerPrefabType prefabType = NetworkScript.Singleton.steamIdToProfileDataDic[steamId].playerPrefabType;

        if (!isNewSpawn
            && oldPosition != null
            && oldRotation != null)
        {

        }

        if (!isNewSpawn
            && oldPosition == null
            || oldRotation == null)
        {
            Debug.LogWarning("Player is not a new spawn, but no old position or rotation was given. Spawning as new.");
        }



        if (playerTypePrefabDic.ContainsKey(prefabType))
        {
            GameObject playerPrefab = playerTypePrefabDic[prefabType];
        }
    }
}
