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

public struct LocalInventoryData
{
    public ItemData[] inventory;
}

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
    public ItemData[] inventory;
}

public struct SaveDataArray
{
    public GameStateData[] gameStateArray;
}

public class SaveManager : NetworkBehaviour
{
    public static SaveManager Singleton {  get; private set; }

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    SavedPlayerSettings defaultPlayerSettings;

    [HideInInspector]
    public SavedPlayerSettings savedPlayerSettings;

    public GameStateData selectedSave;
    public SaveDataArray saveDataArray;

    public LocalInventoryData localInventoryData;

    private string gameSavePath;
    private string backupSavePath;
    private string settingsSavePath;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;

        gameSavePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
        backupSavePath = Path.Combine(Application.persistentDataPath, "backupGameSave.json");
        settingsSavePath = Path.Combine(Application.persistentDataPath, "settings.json");

        LoadSaves();
    }

    void LoadSaves()
    {
        if (File.Exists(gameSavePath))
        {
            string json = File.ReadAllText(gameSavePath);
            saveDataArray = JsonUtility.FromJson<SaveDataArray>(json);
        }
        else
        {
            GameStateData slotZero = new()
            {
                isEmpty = true,
                saveSlot = 0,
                clientDataDic = new(),
                inventory = new ItemData[4],
                ownedItems = new()
            };

            GameStateData slotOne = new()
            {
                isEmpty = true,
                saveSlot = 1,
                clientDataDic = new(),
                inventory = new ItemData[4],
                ownedItems = new()
            };

            GameStateData slotTwo = new()
            {
                isEmpty = true,
                saveSlot = 2,
                clientDataDic = new(),
                inventory = new ItemData[4],
                ownedItems = new()
            };

            GameStateData slotThree = new()
            {
                isEmpty = true,
                saveSlot = 3,
                clientDataDic = new(),
                inventory = new ItemData[4],
                ownedItems = new()
            };

            saveDataArray = new()
            {
                gameStateArray = new GameStateData[4]
            };

            saveDataArray.gameStateArray[0] = slotZero;
            saveDataArray.gameStateArray[1] = slotOne;
            saveDataArray.gameStateArray[2] = slotTwo;
            saveDataArray.gameStateArray[3] = slotThree;
        }

        if (File.Exists(settingsSavePath))
        {
            string json = File.ReadAllText(settingsSavePath);
            SavedPlayerSettings settings = JsonUtility.FromJson<SavedPlayerSettings>(json);

            int i = 0;

            foreach (KeyCodeObj obj in defaultPlayerSettings.keyArray)
            {
                if (settings.keyArray == null)
                {
                    settings = defaultPlayerSettings;
                    break;
                }
                else if (settings.keyArray[i].key == 0)
                {
                    settings.keyArray[i] = obj;
                }

                i++;
            }

            savedPlayerSettings = settings;
        }
        else
        {
            savedPlayerSettings = new()
            {
                keyArray = new KeyCodeObj[19]
            };

            savedPlayerSettings = defaultPlayerSettings;
        }
    }

    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton !=  null)
        {
            if (SceneManager.GetActiveScene().name == "VanScene"
            && NetworkManager.Singleton.IsHost)
            {
                ItemManager.Singleton.AllDropItems();
                SaveGameData();
            }

            if (NetworkManager.Singleton.IsClient)
            {
                ClientSaveData data = SaveLocalClientData();
                SendClientSaveDataServerRpc(data.GetSerializedClientData());
            }
        }
    }

    public void SelectSaveSlot(int slot)
    {
        selectedSave = saveDataArray.gameStateArray[slot];

        gameManager.day.Value = selectedSave.day;
        gameManager.money.Value = selectedSave.money;
    }

    void SaveGameData()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            return;
        }

        RequestAllClientSaveDataClientRpc();

        selectedSave.day = gameManager.day.Value;
        selectedSave.money = gameManager.money.Value;

        selectedSave.playerProfileData = NetworkScript.Singleton.localPlayerProfileData;

        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Player>(out var player))
        {

            player.transform.GetPositionAndRotation(out Vector3 location, out Quaternion rotation);

            selectedSave.location = location;
            selectedSave.rotation = rotation;

            for (int i = 0; i < 4; i++)
            {
                selectedSave.inventory[i] = player.inventory[i];
            }
        }
        else
        {
            Debug.LogError("Unable to find Host PlayerObject while saving.");
            Debug.Assert(false);
        }

        ItemManager.Singleton.PopulateOwnedItems();

        selectedSave.ownedItems = ItemManager.Singleton.ownedItems;

        saveDataArray.gameStateArray[selectedSave.saveSlot] = selectedSave;

        if (File.Exists(gameSavePath))
        {
            File.Copy(gameSavePath, backupSavePath);
        }

        string json = JsonUtility.ToJson(saveDataArray);
        File.WriteAllText(gameSavePath, json);
    }

    public void SaveLocalInventoryData()
    {
        localInventoryData.inventory ??= new ItemData[4];

        if (NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            Debug.LogWarning("No player object to check inventory for.");
            return;
        }

        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Player>(out var player))
        {
            if (player.inventory != null)
            {
                localInventoryData.inventory[0] = player.inventory[0];
                localInventoryData.inventory[1] = player.inventory[1];
                localInventoryData.inventory[2] = player.inventory[2];
                localInventoryData.inventory[3] = player.inventory[3];
            }
        }
        else
        {
            Debug.LogError("Could not find component Player in LocalClient's PlayerObject.");
            Debug.Break();
        }
    }

    public void UpdateAndSaveSettings(SavedPlayerSettings newSettings)
    {
        savedPlayerSettings = newSettings;
        SaveSettingsToJson();
    }

    void SaveSettingsToJson()
    {
        string json = JsonUtility.ToJson(savedPlayerSettings);
        File.WriteAllText(settingsSavePath, json);
    }

    ClientSaveData SaveLocalClientData()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return null;
        }

        float health;
        Vector3 location;
        Quaternion rotation;

        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<Player>(out var player))
        {
            health = player.currentHealth;
            player.transform.GetPositionAndRotation(out location, out rotation);
        }
        else
        {
            PlayerPrefabType prefabType = NetworkScript.Singleton.localPlayerProfileData.playerPrefabType;
            health = PlayerManager.Singleton.playerTypePrefabDic[prefabType].GetComponent<Player>().baseHealth;
            transform.GetPositionAndRotation(out location, out rotation);
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
    public void RequestAllClientSaveDataClientRpc()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return;
        }

        ClientSaveData data = SaveLocalClientData();

        SendClientSaveDataServerRpc(data.GetSerializedClientData());
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void RequestSpecificClientSaveDataRpc(ulong player, RpcParams rpcParams = default)
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return;
        }

        ClientSaveData data = SaveLocalClientData();

        SendClientSaveDataServerRpc(data.GetSerializedClientData());
    }

    [ServerRpc]
    public void SendClientSaveDataServerRpc(SerializedClientSaveData clientData)
    {
        ClientSaveData newData = new();

        newData.GetClientSaveFromSerialized(clientData);

        SaveClientDataToDic(newData);
    }

    void SaveClientDataToDic(ClientSaveData data)
    {
        if (NetworkManager.Singleton.IsClient)
        {
            return;
        }

        selectedSave.clientDataDic ??= new();

        if (data != null)
        {
            ulong steamId = data.playerProfileData.steamID;

            if (selectedSave.clientDataDic.ContainsKey(steamId))
            {
                selectedSave.clientDataDic[steamId] = data;
            }
            else
            {
                selectedSave.clientDataDic.Add(steamId, data);
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

        if (selectedSave.clientDataDic.ContainsKey(steamId))
        {
            saveData = selectedSave.clientDataDic[steamId];
        }

        return saveData;
    }
}
