using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviour
{
    public static NetworkScript Singleton { get; internal set; }

    [SerializeField]
    NetworkManager networkManager;

    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    public event NetworkSceneManager.OnLoadCompleteDelegateHandler OnLoadComplete;

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

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
        networkManager.OnClientConnectedCallback += ClientConnectedCallback;
        networkManager.OnClientDisconnectCallback += ClientDisconnectCallback;
    }

    public void LoadHostGame()
    {
        // this will be reworked when second menu for gathering players is added.
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
    }

    private void ClientDisconnectCallback(ulong clientId)
    {
        OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);
    }
}
