using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviour
{

    [SerializeField]
    private NetworkManager networkManager;

    public void LoadHostGame()
    {
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
}
