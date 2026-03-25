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

    public List<Player> players;
    public List<Player> AlivePlayers => players.Where(p => !p.isDead).ToList();

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

        DontDestroyOnLoad(gameObject);
        InitPlayerPrefabDic();
    }

    private void InitPlayerPrefabDic()
    {
        players = new();
        playerTypePrefabDic = new();
        playerTypeDataDic = new();

        players.Clear();
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
            if (client.PlayerObject != null
                && !client.PlayerObject.GetComponent<Player>().isDead)
            {
                continue;
            }

            if (client.PlayerObject != null
                && client.PlayerObject.GetComponent<Player>().isDead)
            {
                client.PlayerObject.GetComponent<NetworkObject>().Despawn();
            }

            SpawnPlayer(client.ClientId, true, null, null);
        }
    }

    // Player spawn should be controlled by NetworkManager, the RPC is to init the data on clients

    public void SpawnPlayer(ulong clientId, bool isNewSpawn, Vector3? oldPosition, Quaternion? oldRotation)
    {
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.LogWarning("Player spawn error: Client is not connected.");
            return;
        }

        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
        {
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkObject>().Despawn();
        }

        ulong steamId = NetworkScript.Singleton.clientIdToSteamId[clientId];

        if (!NetworkScript.Singleton.steamIdToProfileDataDic.ContainsKey(steamId))
        {
            NetworkScript.Singleton.RequestPlayerProfileDataRpc(clientId, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }

        PlayerPrefabType prefabType = NetworkScript.Singleton.steamIdToProfileDataDic[steamId].playerPrefabType;

        if (prefabType == PlayerPrefabType.Invalid
                    || prefabType == PlayerPrefabType.None
                    || prefabType == PlayerPrefabType.Max
                    || !playerTypePrefabDic.ContainsKey(prefabType))
        {
            prefabType = PlayerPrefabType.Basic;
            Debug.LogWarning("No profile data for client " + clientId + ", using Basic player type.");
        }

        GameObject playerPrefab = playerTypePrefabDic[prefabType];
        Vector3 location = spawnNode.GiveSpawnLocation(playerPrefab);
        Quaternion quaternion = Quaternion.identity;

        if (isNewSpawn)
        {
            Debug.Log("this is a new spawn");
        }

        if (!isNewSpawn
            && oldPosition == null)
        {
            Debug.LogWarning("Player is not a new spawn, but errors with old position. Spawning as new.");
        }

        if (!isNewSpawn
            && oldPosition != null
            && oldRotation != null)
        {
            location = (Vector3)oldPosition;
            quaternion = (Quaternion)oldRotation;
        }

        playerPrefab = Instantiate(playerPrefab, location, quaternion);

        if (playerPrefab.TryGetComponent<NetworkObject>(out var netObj))
        {
            netObj.SpawnAsPlayerObject(clientId, false);

            if (playerPrefab.TryGetComponent<Player>(out var player)
                && !players.Contains(player))
            {
                players.Add(player);
            }
            else
            {
                Debug.LogError("Player prefab does not have a Player component or already is in 'players'.");
            }
        }
    }

    public void SetAllPlayersDead(bool isFiltered, List<ulong> safePlayers)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            // this will be edited later when dead player following is added
            Player player = client.PlayerObject.GetComponent<Player>();

            if (isFiltered)
            {
                player.isDead = true;
            }

            if (safePlayers != null
                && !safePlayers.Contains(client.ClientId)
                && !isFiltered)
            {
                player.isDead = true;
            }
        }
    }

    public Player GetPlayerBySteamID(ulong steamID)
    {
        NetworkScript.Singleton.steamIdToClientId.TryGetValue(steamID, out ulong clientId);

        return NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
    }
}
