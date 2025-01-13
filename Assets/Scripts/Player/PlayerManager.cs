using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerData> playerList = new();

    private void Awake()
    {
        ResetPlayerList();
    }

    private void Update()
    {
        
    }

    public void SpawnPlayer(PlayerData playerData, Vector3 location, Quaternion quaternion)
    {
        if (playerData != null 
            && !FindPlayerName(playerData))
        {
            GameObject newPlayer = Instantiate(playerData.playerPrefab, location, quaternion);

            GameObject newCamera = Instantiate(playerData.cameraPrefab, location, quaternion);

            if (newPlayer.TryGetComponent<Player>(out var playerComponent))
            {
                playerComponent.PlayerInit(playerData, playerList.Count);

                playerComponent.CameraInit(newCamera, newPlayer);
            }
            else
            {
                Debug.LogError("Error getting component 'Player' when spawning player");
                Debug.Break();
            }

            playerList.Add(playerData);
        }
        else if (FindPlayerName(playerData))
        {
            Debug.LogError($"SpawnPlayer: There is already a player with this name.");
        }
        else
        {
            Debug.LogError($"SpawnPlayer: playerData is {playerData}");
        }
    }

    public bool FindPlayerName(PlayerData playerData)
    {
        if (playerList.Count != 0)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].name == playerData.name)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ResetPlayerList()
    {
        if (playerList != null)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                Destroy(playerList[i]);
            }
        }
    }

    public void PlayerListUpdate(PlayerData playerData)
    {
        int playerNumber = playerData.playerNumber;
    }
}
