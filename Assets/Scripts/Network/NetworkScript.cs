using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;
using Unity.Collections;

public enum ConnectionStatus
{
    Connected,
    Disconnected
}

public struct PlayerProfileData : INetworkSerializable
{
    public ulong steamID;
    public FixedString64Bytes playerName;
    public PlayerPrefabType playerPrefabType;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref steamID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerPrefabType);
    }
}

public class NetworkScript : MonoBehaviour
{
    public static NetworkScript Singleton { get; internal set; }

    [SerializeField]
    NetworkManager networkManager;

    public PlayerProfileData localPlayerProfileData;

    public List<PlayerProfileData> allPlayerProfileData;

    public Dictionary<ulong, PlayerProfileData> steamIdToProfileDataDic;
    public Dictionary<ulong, ulong> clientIdToSteamId;
    public Dictionary<ulong, ulong> steamIdToClientId;

    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            networkManager.OnClientConnectedCallback += ClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += ClientDisconnectCallback;
        }
        else
        {
            Debug.LogError("NetworkScript: SteamManager not initialized");
            Debug.Break();
        }
    }

    private void InitPlayerProfileList()
    {
        allPlayerProfileData = new();
        allPlayerProfileData.Clear();

        Debug.Log("Initialized allPlayerProfileData list");

        if (networkManager.IsHost)
        {
            steamIdToProfileDataDic = new();
            steamIdToProfileDataDic.Clear();

            localPlayerProfileData = GetLocalPlayerProfileData(networkManager.LocalClientId);

            allPlayerProfileData.Add(localPlayerProfileData);

            steamIdToProfileDataDic.Add(localPlayerProfileData.steamID, localPlayerProfileData);

            Debug.Log($"Added local player {localPlayerProfileData.playerName} to allPlayerProfileData");
        }
        else
        {
            Debug.Log("Not server, waiting for profile data from server...");
        }
    }

    private void InitSteamClientIdDic()
    {
        clientIdToSteamId = new Dictionary<ulong, ulong>();
        steamIdToClientId = new Dictionary<ulong, ulong>();

        clientIdToSteamId.Clear();
        steamIdToClientId.Clear();

        ConnectClientAndSteamId(networkManager.LocalClientId, localPlayerProfileData.steamID);
    }

    public void LoadHostGame()
    {
        networkManager.NetworkConfig.ConnectionApproval = true;
        networkManager.ConnectionApprovalCallback = ApprovalCheck;
        networkManager.StartHost();

        InitPlayerProfileList();
        InitSteamClientIdDic();

        networkManager.SceneManager.OnLoadComplete += HandleLoadComplete;
        networkManager.SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
    }

    // Add item save functionality to these
    public void LoadVanScene()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            EnemyManager.Singleton.DestroyAllEnemies();
        }

        networkManager.SceneManager.LoadScene("VanScene", LoadSceneMode.Single);

        SaveManager.Singleton.SaveGameData();
    }

    public void LoadGameScene()
    {
        GameManager.Singleton.GenerateNewGameInfoServerRpc();
        networkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void LoadShopScene()
    {
        networkManager.SceneManager.LoadScene("ShopScene", LoadSceneMode.Single);
    }

    public void LoadClient()
    {
        networkManager.StartClient();

        InitPlayerProfileList();
        networkManager.SceneManager.OnLoadComplete += HandleLoadComplete;
    }

    public void LoadServer()
    {
        networkManager.StartServer();
    }

    public void Disconnect()
    {
        networkManager.SceneManager.OnLoadComplete -= HandleLoadComplete;
        networkManager.Shutdown();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;

        response.Pending = false;
    }

    private void ConnectClientAndSteamId(ulong clientId, ulong steamId)
    {
        if (clientIdToSteamId.ContainsKey(clientId))
        {
            Debug.LogError($"ClientID {clientId} is already connected to SteamID {clientIdToSteamId[clientId]}");
            return;
        }
        else if (steamIdToClientId.ContainsKey(steamId))
        {
            Debug.LogError($"SteamID {steamId} is already connected to ClientID {steamIdToClientId[steamId]}");
            return;
        }
        
        clientIdToSteamId.Add(clientId, steamId);
        steamIdToClientId.Add(steamId, clientId);
    }

    private void DisconnectClientAndSteamId(ulong clientId)
    {
        if (clientIdToSteamId.TryGetValue(clientId, out ulong steamId))
        {
            clientIdToSteamId.Remove(clientId);
            steamIdToClientId.Remove(steamId);
        }
        else
        {
            Debug.LogError($"ClientID {clientId} not found in clientIdToSteamId dictionary");
        }
    }

    private void OnDestroy()
    {
        if (Singleton != null)
        {
            networkManager.OnClientConnectedCallback -= ClientConnectedCallback;
            networkManager.OnClientDisconnectCallback -= ClientDisconnectCallback;
        }
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);

        RequestPlayerProfileDataRpc(clientId);
    }

    private void ClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);

        if (networkManager.IsHost
            && clientId == networkManager.LocalClientId)
        {
            Debug.Log("Host disconnected.");
            return;
        }

        PlayerProfileData? profileData = allPlayerProfileData.Find(p => steamIdToClientId.ContainsKey(p.steamID) && steamIdToClientId[p.steamID] == clientId);

        DisconnectClientAndSteamId(clientId);

        if (profileData.HasValue)
        {
            NotifyClientDisconnectedRpc(profileData.Value);
            allPlayerProfileData.Remove(profileData.Value);
            NetworkUIScript.Singleton.RemovePlayerFromDic(profileData.Value);
        }
        else
        {
            Debug.LogError($"No profile data found for disconnected clientId {clientId}");
        }
    }

    private void HandleLoadComplete(ulong player, string sceneName, LoadSceneMode loadSceneMode)
    {
        NetworkUIScript.Singleton.PlayerListSceneCheck();

        if (MapManager.Singleton != null)
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            MapManager.Singleton.InitMapMan();
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void RequestPlayerProfileDataRpc(ulong clientId, RpcParams rpcParams = default)
    {
        PlayerProfileData profileData = new();

        if (clientId == networkManager.LocalClientId)
        {
            profileData = GetLocalPlayerProfileData(clientId);
        }
        else
        {
            Debug.LogError("RequestPlayerProfileDataRpc called for non-local client");
            Debug.Break();
        }

        localPlayerProfileData = profileData;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendClientPlayerListRpc(ulong clientId, List<PlayerProfileData> playerList, RpcParams rpcParams = default)
    {
        if (allPlayerProfileData == null)
        {
            InitPlayerProfileList();
        }

        foreach (PlayerProfileData data in playerList)
        {
            allPlayerProfileData.Add(data);
        }

        NetworkUIScript.Singleton.InitPlayerDic();
    }

    private PlayerProfileData GetLocalPlayerProfileData(ulong clientId)
    {
        PlayerProfileData profileData = new()
        {
            steamID = SteamUser.GetSteamID().m_SteamID,
            playerName = SteamFriends.GetPersonaName(),
            // REMEMBER TO ADD SETTINGS TO START IN BOOTSTRAP
        };

        if (!networkManager.IsHost)
        {
            ReceiveClientProfileDataRpc(clientId, profileData);
        }

        return profileData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReceiveClientProfileDataRpc(ulong clientId, PlayerProfileData profileData)
    {
        ConnectClientAndSteamId(clientId, profileData.steamID);

        if (allPlayerProfileData == null)
        {
            InitPlayerProfileList();
        }
        else if (allPlayerProfileData.Exists(p => p.steamID == profileData.steamID))
        {
            Debug.LogError($"Player with SteamID {profileData.steamID} already exists in allPlayerProfileData");
            return;
        }

        if (!steamIdToProfileDataDic.ContainsKey(profileData.steamID))
        {
            steamIdToProfileDataDic.Add(profileData.steamID, profileData);
        }
        else
        {
            Debug.LogError($"Player with SteamID {profileData.steamID} already exists in steamIdToProfileDataDic");
            return;
        }

        SendClientPlayerListRpc(clientId, allPlayerProfileData);
        allPlayerProfileData.Add(profileData);
        NetworkUIScript.Singleton.AddPlayerToDic(profileData);
        SendAllPlayerProfileDataRpc(profileData);
    }

    [ClientRpc]
    private void SendAllPlayerProfileDataRpc(PlayerProfileData profileData)
    {
        allPlayerProfileData.Add(profileData);
        NetworkUIScript.Singleton.AddPlayerToDic(profileData);
    }

    [ClientRpc]
    private void NotifyClientDisconnectedRpc(PlayerProfileData profileData)
    {
        allPlayerProfileData.Remove(profileData);
        NetworkUIScript.Singleton.RemovePlayerFromDic(profileData);
    }

    [ServerRpc]
    public void RequestDestinationServerRpc(VanDestination destination)
    {
        switch (destination)
        {
            case VanDestination.Van:
                LoadVanScene();
                break;
            case VanDestination.Game:
                LoadGameScene();
                break;
            case VanDestination.Shop:
                LoadShopScene();
                break;
            default:
                Debug.LogError("Non-implimented destination: " + destination);
                Debug.Break();
                break;
        }
    }
}
