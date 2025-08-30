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
    Max
}

[System.Serializable]
public class PlayerPrefabData
{
    public PlayerPrefabType playerPrefabType;
    public GameObject playerPrefab;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public List<PlayerPrefabData> playerPrefabDataList = new();

    Dictionary<PlayerPrefabType, GameObject> playerPrefabDic = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitPlayerPrefabDic();
    }

    private void InitPlayerPrefabDic()
    {
        playerPrefabDic.Clear();

        foreach (PlayerPrefabData data in playerPrefabDataList)
        {
            if (data.playerPrefabType == PlayerPrefabType.Invalid
                || data.playerPrefabType == PlayerPrefabType.None
                || data.playerPrefabType == PlayerPrefabType.Max)
            {
                Debug.LogWarning($"Skipping invalid or none player prefab type: {data.playerPrefabType}");
                continue;
            }

            if (playerPrefabDic.ContainsKey(data.playerPrefabType))
            {
                Debug.LogWarning($"Dictionary already contains the key: {data.playerPrefabType}");
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
    public void SpawnPlayerRpc(ulong player, PlayerPrefabType prefab, bool isNewSpawn, Vector3 oldPosition, Quaternion oldOrientation, RpcParams rpcParams = default)
    {
        if (playerPrefabDic.ContainsKey(prefab))
        {
            GameObject playerPrefab = playerPrefabDic[prefab];


        }
    }



    //public PlayerData basePlayerData;
    //public List<PlayerData> playerList;
    //public PlayerData[] playerArray;
    //// Temporarily here. May be moved higher up in the future?
    //public bool canAddPlayers = true;

    //private void Awake()
    //{
    //    CreatePlayerList();
    //}

    //private void Update()
    //{
    //    // Add function in the future for when in a joinable lobby, checking for players and adding them to the list is updating
    //}

    //public void CreatePlayerList()
    //{
    //    if (playerList != null)
    //    {
    //        playerList.Clear();
    //    }
    //    else
    //    {
    //        playerList = new List<PlayerData>();
    //    }
    //}

    //public void AddPlayerToList(PlayerData playerData)
    //{
    //    if (playerData != null 
    //        && playerList != null
    //        && canAddPlayers 
    //        && !FindPlayerName(playerData))
    //    {
    //        playerList.Add(playerData);
    //    }
    //    else if (playerList == null)
    //    {
    //        playerList = new List<PlayerData>
    //        {
    //            playerData
    //        };
    //    }
    //    else if (!canAddPlayers)
    //    {
    //        Debug.LogError("AddPlayerToList error: you can't add players right now.");
    //        Debug.Break();
    //    }
    //    else if (FindPlayerName(playerData))
    //    {
    //        Debug.LogError("AddPlayerToList error: This player name already exists.");
    //    }
    //    else
    //    {
    //        Debug.LogError("AddPlayerToList error: catch all");
    //        Debug.Break();
    //    }
    //}

    //public void PlayerListToArray()
    //{
    //   if (playerList != null 
    //        && !canAddPlayers)
    //   {
    //        playerArray = playerList.ToArray();
    //        playerList.Clear();
    //   }
    //   else
    //   {
    //        Debug.LogError("playerList is null");
    //        Debug.Break();
    //   }
    //}

    //public void PlayerArrayToList()
    //{
    //    if (playerArray != null 
    //        && canAddPlayers)
    //    {
    //        playerList = playerArray
    //            .Where(player => player != null)
    //            .ToList();
    //        playerArray = null;
    //    }
    //    else
    //    {
    //        Debug.LogError("playerArray is null");
    //        Debug.Break();
    //    }
    //}

    //public bool FindPlayerName(PlayerData playerData)
    //{
    //    if (playerList.Count != 0)
    //    {
    //        for (int i = 0; i < playerList.Count; i++)
    //        {
    //            if (playerList[i].playerName == playerData.playerName)
    //            {
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}

    //public void PlayerUpdate(PlayerData playerData)
    //{
    //    if (playerArray != null 
    //        && playerData != null)
    //    {
    //        for (int i = 0; i < playerArray.Length; i++)
    //        {
    //            if (playerArray[i].playerName == playerData.playerName)
    //            {
    //                playerArray[i] = playerData;
    //                Debug.Log("playerData updated");
    //                return;
    //            }
    //        }
    //    } 
    //    else if (playerList.Count > 0
    //             && playerData != null)
    //    {
    //        for (int i = 0; i < playerList.Count; i++)
    //        {
    //            if (playerList[i].playerName == playerData.playerName)
    //            {
    //                playerList[i] = playerData;
    //                Debug.Log("playerData updated");
    //                return;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("playerArray and playerList are either null or playerData is null");
    //        Debug.Break();
    //    }
    //}
}
