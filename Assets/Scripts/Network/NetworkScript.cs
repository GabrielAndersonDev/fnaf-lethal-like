using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviour
{
    public static NetworkScript Singleton { get; internal set; }

    [SerializeField]
    private NetworkManager networkManager;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    public void LoadHostGame(GameInfo gameInfo)
    {
        networkManager.StartHost();
        GameManager.Instance.gameInfo.Value = gameInfo;
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

    public void StopHost()
    {
        networkManager.Shutdown();
    }

    public void StopClient(Player player)
    {
        networkManager.DisconnectClient(player.OwnerClientId);
    }
}
