using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public enum ConnectionStatus
{
    Connected,
    Disconnected
}

public class NetworkScript : MonoBehaviour
{
    public static NetworkScript Singleton { get; internal set; }

    [SerializeField]
    NetworkManager networkManager;

    Dictionary<ulong, CSteamID> clientIdToSteamId;
    Dictionary<CSteamID, ulong> steamIdToClientId;

    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    public event NetworkSceneManager.OnLoadCompleteDelegateHandler OnLoadComplete;

    protected Callback<PersonaStateChange_t> m_PersonaStateChange;
    protected Callback<AvatarImageLoaded_t> m_AvatarImageLoaded;

    private CSteamID m_Friend;
    private string m_Name;
    private Texture2D m_Avatar;

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
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
            CSteamID id = SteamUser.GetSteamID();
            Debug.Log(id.ToString());

            networkManager.OnClientConnectedCallback += ClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += ClientDisconnectCallback;
            OnLoadComplete += HandleLoadComplete;
        }
    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
            m_AvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        }
    }

    void OnPersonaStateChange(PersonaStateChange_t pCallback)
    {
        Debug.Log("[" + PersonaStateChange_t.k_iCallback + " - PersonaStateChange] - " + pCallback.m_ulSteamID + " -- " + pCallback.m_nChangeFlags);
    }

    void OnAvatarImageLoaded(AvatarImageLoaded_t pCallback)
    {
        Debug.Log("[" + AvatarImageLoaded_t.k_iCallback + " - AvatarImageLoaded] - " + pCallback.m_steamID + " -- " + pCallback.m_iImage + " -- " + pCallback.m_iWide + " -- " + pCallback.m_iTall);
    }

    public void LoadHostGame()
    {
        // this will be reworked when second menu for gathering players is added.
        networkManager.ConnectionApprovalCallback = ApprovalCheck;

        networkManager.StartHost();
        networkManager.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void LoadClient()
    {
        networkManager.StartClient();
    }

    public void LoadServer()
    {
        networkManager.StartServer();
    }

    public void Disconnect()
    {
        networkManager.Shutdown();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // SteamAPICall_t handle = SteamFriends.GetPersonaName()

        response.Approved = true;
        response.CreatePlayerObject = false;

        response.Pending = false;
    }

    private void SetPersonaName(PersonaStateChange_t personaStateChange_t)
    {
        
    }

    private void ConnectClientAndSteamId(ulong clientId, CSteamID steamId)
    {

    }

    private void OnDestroy()
    {
        if (Singleton != null)
        {
            networkManager.OnClientConnectedCallback -= ClientConnectedCallback;
            networkManager.OnClientDisconnectCallback -= ClientDisconnectCallback;
            OnLoadComplete -= HandleLoadComplete;
        }
    }

    private void OnSteamID(CSteamID steamID, bool failure)
    {
        if (failure)
        {
            Debug.Log("There was an error getting the steamID");
        }
        else
        {
            Debug.Log(steamID.ToString());
        }
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);
    }

    private void ClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);
    }

    private void HandleLoadComplete(ulong player, string sceneName, LoadSceneMode loadSceneMode)
    {
        OnLoadComplete?.Invoke(player, sceneName, loadSceneMode);

        NetworkUIScript.Singleton.PlayerListSceneCheck(sceneName);

        
    }
}
