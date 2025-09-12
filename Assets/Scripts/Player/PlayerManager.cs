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

    [SerializeField]
    List<PlayerTypePrefabObj> playerTypePrefabObjList = new();

    public Dictionary<PlayerPrefabType, GameObject> playerTypePrefabDic = new();
    public Dictionary<PlayerPrefabType, PlayerData> playerTypeDataDic = new();

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
    }

    private void Start()
    {
        InitPlayerPrefabDic();
    }

    private void InitPlayerPrefabDic()
    {
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
                continue;
            }

            playerPrefabDic.Add(data.playerPrefabType, data.playerPrefab);
            Debug.Log($"Added player prefab to playerPrefabDic under key: {data.playerPrefabType}");
        }
    }


    public void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                // Player already spawned
                continue;
            }


        }
    }

    // Player spawn should be controlled by NetworkManager, the RPC is to init the data on clients

    [Rpc(SendTo.SpecifiedInParams)]
    public void SpawnPlayerRpc(ulong player, PlayerPrefabType prefabType, bool isNewSpawn, Vector3 oldPosition, Quaternion oldRotation, RpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(player)
            || NetworkManager.Singleton.ConnectedClients[player].PlayerObject != null)
        {
            Debug.LogWarning("Player spawn error: Client is not connected or already has a PlayerObject.");
            return;
        }

        if (playerTypePrefabDic.ContainsKey(prefabType))
        {
            GameObject playerPrefab = playerTypePrefabDic[prefabType];
        }
    }
}
