using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScript : MonoBehaviour
{

    [SerializeField]
    private NetworkManager networkManager;

    NetworkVariable<int> seed = new(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<bool> useRandomSeed = new(writePerm: NetworkVariableWritePermission.Owner);

    public void LoadHostGame(bool useRandSeed, int sentSeed)
    {
        useRandomSeed.Value = useRandSeed;

        if (useRandSeed)
        {
            seed.Value = Random.Range(0, 100000);
        }
        else
        {

        }

        Random.InitState(seed.Value);

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
