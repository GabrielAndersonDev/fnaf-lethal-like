using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct KeyCodeObj
{
    public string name;
    public KeyCode key;
}

[System.Serializable]
public struct SavedPlayerSettings
{
    public PlayerPrefabType playerPrefabType;

    public KeyCodeObj[] keyArray;

    public bool isTogglePlayerList;
    public bool isToggleSprint;
    public bool isToggleCrouch;
}

[System.Serializable]
public struct GameStateData
{
    public bool isEmpty;
    public int saveSlot;
    public int day;
    public int money;

    public PlayerProfileData playerProfileData;

    public Dictionary<ulong, ClientSaveData> clientDataDic;

    public Vector3 location;
    public Quaternion rotation;

    public List<OwnedItemObj> ownedItems;
}

[System.Serializable]
public struct SaveDataArray
{
    public GameStateData[] gameStateArray;
}

public class SaveManager
{
    public static GameManager GameManager = GameManager.Singleton;

    public static string GetGameSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "gameSave.json");
    }

    public static string GetBackupSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "backupGameSave.json");
    }

    public static string GetSettingsSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "settings.json");
    }

    public static void LoadSaves()
    {
        if (File.Exists(GetGameSavePath()))
        {
            string json = File.ReadAllText(GetGameSavePath());
            GameManager.saveDataArray = JsonUtility.FromJson<SaveDataArray>(json);
        }
        else
        {
            GameStateData slotZero = new()
            {
                isEmpty = true,
                saveSlot = 0,
                clientDataDic = new(),
                ownedItems = new()
            };

            GameStateData slotOne = new()
            {
                isEmpty = true,
                saveSlot = 1,
                clientDataDic = new(),
                ownedItems = new()
            };

            GameStateData slotTwo = new()
            {
                isEmpty = true,
                saveSlot = 2,
                clientDataDic = new(),
                ownedItems = new()
            };

            GameStateData slotThree = new()
            {
                isEmpty = true,
                saveSlot = 3,
                clientDataDic = new(),
                ownedItems = new()
            };

            GameManager.saveDataArray.gameStateArray = new GameStateData[4];

            GameManager.saveDataArray.gameStateArray[0] = slotZero;
            GameManager.saveDataArray.gameStateArray[1] = slotOne;
            GameManager.saveDataArray.gameStateArray[2] = slotTwo;
            GameManager.saveDataArray.gameStateArray[3] = slotThree;
        }

        if (File.Exists(GetSettingsSavePath()))
        {
            string json = File.ReadAllText(GetSettingsSavePath());
            SavedPlayerSettings settings = JsonUtility.FromJson<SavedPlayerSettings>(json);

            int i = 0;

            Debug.Log("settings save file does exist");

            if (settings.keyArray == null)
            {
                GameManager.playerSettings = GameManager.defaultPlayerSettings;
                return;
            }

            foreach (KeyCodeObj obj in GameManager.playerSettings.keyArray)
            {
                if (settings.keyArray[i].key == 0)
                {
                    settings.keyArray[i] = obj;
                }

                i++;
            }

            GameManager.playerSettings = settings;
        }
        else
        {
            Debug.Log("settings save file does NOT exist");

            GameManager.playerSettings = new()
            {
                keyArray = new KeyCodeObj[19]
            };

            GameManager.playerSettings = GameManager.defaultPlayerSettings;
            Debug.Log(GameManager.playerSettings);
        }
    }

    public static void SelectSaveSlot(int slot)
    {
        GameManager.selectedSave = GameManager.saveDataArray.gameStateArray[slot];
    }

    public static void DeleteSaveSlot(int slot)
    {
        GameStateData newEmptySave = new()
        {
            isEmpty = true,
            saveSlot = slot,
            clientDataDic = new(),
            ownedItems = new()
        };
        GameManager.saveDataArray.gameStateArray[slot] = newEmptySave;
        SaveGameData();
    }

    public static void SaveGameData()
    {
        if (!NetworkManager.Singleton.IsHost
            || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name != "VanScene")
        {
            Debug.LogWarning("Can only save from VanScene.");
            return;
        }

        Debug.Log("saving game data");

        RequestAllClientSaveDataClientRpc();

        GameManager.selectedSave.day = GameManager.day.Value;
        GameManager.selectedSave.money = GameManager.money.Value;

        GameManager.selectedSave.playerProfileData = NetworkScript.Singleton.localPlayerProfileData;

        GameManager.selectedSave.ownedItems ??= new();

        GameManager.selectedSave.ownedItems.Clear();

        foreach (OwnedItemObj item in ItemManager.Singleton.ownedItems)
        {
            if (!GameManager.selectedSave.ownedItems.Contains(item))
            {
                GameManager.selectedSave.ownedItems.Add(item);
            }
        }

        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Player>(out var player))
        {
            player.transform.GetPositionAndRotation(out Vector3 location, out Quaternion rotation);

            GameManager.selectedSave.location = location;
            GameManager.selectedSave.rotation = rotation;
        }
        else
        {
            Debug.LogError("Unable to find Host PlayerObject while saving.");
            Debug.Assert(false);
        }

        GameManager.selectedSave.isEmpty = false;

        GameManager.saveDataArray.gameStateArray[GameManager.selectedSave.saveSlot] = GameManager.selectedSave;

        if (File.Exists(GetGameSavePath()))
        {
            if (File.Exists(GetBackupSavePath()))
            {
                File.Delete(GetBackupSavePath());
            }
            
            File.Copy(GetGameSavePath(), GetBackupSavePath());
        }

        string json = JsonUtility.ToJson(GameManager.saveDataArray);
        File.WriteAllText(GetGameSavePath(), json);
    }

    public static void SaveSettingsToJson()
    {
        string json = JsonUtility.ToJson(GameManager.playerSettings);
        File.WriteAllText(GetSettingsSavePath(), json);
    }

    public static ClientSaveData SaveLocalClientData()
    {
        if (NetworkManager.Singleton.IsHost
            || NetworkManager.Singleton.IsServer)
        {
            return null;
        }

        float health;
        Vector3 location = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out Player player))
        {
            health = player.currentHealth;
            player.transform.GetPositionAndRotation(out location, out rotation);
        }
        else
        {
            PlayerPrefabType prefabType = NetworkScript.Singleton.localPlayerProfileData.playerPrefabType;
            health = PlayerManager.Singleton.playerTypePrefabDic[prefabType].GetComponent<Player>().baseHealth;
        }

        ClientSaveData clientData = new()
        {
            sessionSeed = GameManager.Singleton.sessionSeed.Value,
            playerProfileData = NetworkScript.Singleton.localPlayerProfileData,
            health = health,
            location = location,
            rotation = rotation,
            isDead = false
        };

        return clientData;
    }

    [ClientRpc]
    public static void RequestAllClientSaveDataClientRpc()
    {
        if (NetworkManager.Singleton.IsHost
            || NetworkManager.Singleton.IsServer)
        {
            return;
        }

        ClientSaveData data = SaveLocalClientData();

        SendClientSaveDataServerRpc(data.GetSerializedClientData());
    }

    [Rpc(SendTo.SpecifiedInParams)]
    static void RequestSpecificClientSaveDataRpc(ulong player, RpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.IsHost 
            || NetworkManager.Singleton.IsServer)
        {
            return;
        }

        ClientSaveData data = SaveLocalClientData();

        SendClientSaveDataServerRpc(data.GetSerializedClientData());
    }

    [ServerRpc]
    public static void SendClientSaveDataServerRpc(SerializedClientSaveData clientData)
    {
        ClientSaveData newData = new();

        newData.GetClientSaveFromSerialized(clientData);

        SaveClientDataToDic(newData);
    }

    static void SaveClientDataToDic(ClientSaveData data)
    {
        if (!NetworkManager.Singleton.IsHost
            || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        GameManager.selectedSave.clientDataDic ??= new();

        if (data != null)
        {
            ulong steamId = data.playerProfileData.steamID;

            if (GameManager.selectedSave.clientDataDic.ContainsKey(steamId))
            {
                GameManager.selectedSave.clientDataDic[steamId] = data;
            }
            else
            {
                GameManager.selectedSave.clientDataDic.Add(steamId, data);
            }
        }
        else
        {
            Debug.Assert(false);
        }
    }

    public ClientSaveData RetrieveClientSaveData(ulong clientId)
    {
        ClientSaveData saveData = null;

        ulong steamId = NetworkScript.Singleton.clientIdToSteamId[clientId];

        if (GameManager.selectedSave.clientDataDic.ContainsKey(steamId))
        {
            saveData = GameManager.selectedSave.clientDataDic[steamId];
        }

        return saveData;
    }
}
