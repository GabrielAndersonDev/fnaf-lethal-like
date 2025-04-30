using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerData basePlayerData;
    public List<PlayerData> playerList;
    public PlayerData[] playerArray;
    // Temporarily here. May be moved higher up in the future?
    public bool canAddPlayers = true;

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

    public void AddPlayerToList(PlayerData playerData)
    {
        if (playerData != null 
            && playerList != null
            && canAddPlayers 
            && !FindPlayerName(playerData))
        {
            playerList.Add(playerData);
        }
        else if (playerList == null)
        {
            playerList = new List<PlayerData>
            {
                playerData
            };
        }
        else if (!canAddPlayers)
        {
            Debug.LogError("AddPlayerToList error: you can't add players right now.");
            Debug.Break();
        }
        else if (FindPlayerName(playerData))
        {
            Debug.LogError("AddPlayerToList error: This player name already exists.");
        }
        else
        {
            Debug.LogError("AddPlayerToList error: catch all");
            Debug.Break();
        }
    }

    public void PlayerListToArray()
    {
       if (playerList != null 
            && !canAddPlayers)
       {
            playerArray = playerList.ToArray();
            playerList.Clear();
       }
       else
       {
            Debug.LogError("playerList is null");
            Debug.Break();
       }
    }

    public void PlayerArrayToList()
    {
        if (playerArray != null 
            && canAddPlayers)
        {
            playerList = playerArray
                .Where(player => player != null)
                .ToList();
            playerArray = null;
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

            Camera povCam = newCamera.GetComponentInChildren<Camera>();

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
                if (playerList[i].playerName == playerData.playerName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void PlayerUpdate(PlayerData playerData)
    {
        if (playerArray != null 
            && playerData != null)
        {
            for (int i = 0; i < playerArray.Length; i++)
            {
                if (playerArray[i].playerName == playerData.playerName)
                {
                    playerArray[i] = playerData;
                    Debug.Log("playerData updated");
                    return;
                }
            }
        } 
        else if (playerList.Count > 0
                 && playerData != null)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].playerName == playerData.playerName)
                {
                    playerList[i] = playerData;
                    Debug.Log("playerData updated");
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("playerArray and playerList are either null or playerData is null");
            Debug.Break();
        }
    }
}
