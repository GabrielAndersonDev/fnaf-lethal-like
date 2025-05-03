using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviour
{

    [SerializeField]
    private NetworkManager networkManager;

    private void Update()
    {
        if (networkManager.IsServer || networkManager.IsClient || networkManager.IsHost)
        {
            SubmitNewPosition();
        }
    }

    public void LoadHostGame()
    {
        SceneManager.LoadScene("GameScene");
        networkManager.StartHost();

        Debug.Log(networkManager.SpawnManager.GetLocalPlayerObject().name);
    }

    public void LoadClient()
    {
        networkManager.StartClient();
    }

    public void LoadServer()
    {
        networkManager.StartServer();
    }

    private void SubmitNewPosition()    
    {
        if (networkManager.SpawnManager.GetLocalPlayerObject() != null)
        {
            if (networkManager.IsServer && !networkManager.IsClient)
            {
                foreach (ulong uid in networkManager.ConnectedClientsIds)
                    networkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().MovePlayer();
            }
            else
            {
                var playerObject = networkManager.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<Player>();
                player.MovePlayer();
            }
        }
    }
}
