using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerData> playerList;
    public PlayerData[] playerArray;

    private void Awake()
    {
        CreatePlayerList();

    }

    private void Update()
    {
        // Add function in the future for when in a joinable lobby, checking for players and adding them to the list is updating
    }

    public void CreatePlayerList()
    {
        if (playerList != null)
        {
            playerList.Clear();
        }
        else
        {
            playerList = new List<PlayerData>();
        }
    }

    public void PlayerListToArray()
    {
       if (playerList != null)
       {
            playerArray = playerList.ToArray();
       }
       else
       {
            Debug.LogError("playerList is null");
            Debug.Break();
       }
    }

    public void PlayerArrayToList()
    {
        if (playerArray != null)
        {
            foreach (PlayerData player in playerArray)
            {
            
            }
        }
        else
        {
            Debug.LogError("playerArray is null");
            Debug.Break();
        }
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

    public void PlayerListUpdate(PlayerData playerData)
    {
        int playerNumber = playerData.playerNumber;
    }
}
