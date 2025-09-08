using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class SavedPlayerSettings
{
    public PlayerPrefabType playerPrefabType;

    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode useKey;
    public KeyCode attackKey;
    public KeyCode interactKey;
    public KeyCode dropKey;
    public KeyCode alternateKey;
    public KeyCode lightKey;
    public KeyCode pauseKey;
    public KeyCode inventorySlotOne;
    public KeyCode inventorySlotTwo;
    public KeyCode inventorySlotThree;
    public KeyCode inventorySlotFour;
    public KeyCode playerListKey;
}

public class ClientSaveData
{
    public int sessionSeed;  // int for double-checking session. re-creates each load before proper seeding works.
    public PlayerProfileData playerProfileData;
    public float health;
    public Vector3 location;
    public Quaternion rotation;
    public bool isDead;
}

public class GameStateData
{
    public int saveSlot;
    public int day;
    public int money;

    public PlayerProfileData playerProfileData;

    public List<ClientSaveData> clientSaveDataList = new();

    public Vector3 location;
    public Quaternion rotation;

    public List<OwnedItemObj> ownedItems = new();
    public ItemData[] inventory = new ItemData[4];
}

public class SaveDataArray
{
    public GameStateData[] gameStateArray = new GameStateData[4];
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
            saveDataArray = new SaveDataArray();

            for (int i = 0; i < 4; i++)
            {
                saveDataArray.gameStateArray[i].saveSlot = i;
            }
        }

        if (File.Exists(settingsSavePath))
        {
            string json = File.ReadAllText(settingsSavePath);
            savedPlayerSettings = JsonUtility.FromJson<SavedPlayerSettings>(json);
        }
        else
        {
            savedPlayerSettings = defaultPlayerSettings;
        }
    }

    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton !=  null)
        {
            if (NetworkScript.Singleton.currentScene == "VanScene"
            && NetworkManager.Singleton.IsHost)
            {
                ItemManager.Singleton.AllDropItemsClientRpc();
                SaveGameData();
            }

            if (NetworkManager.Singleton.IsClient)
            {
                SendClientSaveDataServerRpc();
            }
        }

        SaveSettingsToJson();
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

    void SaveSettingsToJson()
    {
        string json = JsonUtility.ToJson(savedPlayerSettings);
        File.WriteAllText(settingsSavePath, json);
    }

    [ClientRpc]
    public void RequestAllClientSaveDataClientRpc()
    {
        SendClientSaveDataServerRpc();
    }

    [ServerRpc]
    public void SendClientSaveDataServerRpc()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return;
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
            health = PlayerManager.Singleton.playerTypeDataDic[prefabType].baseHealth;
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

        SaveClientData(clientData);
    }

    void SaveClientData(ClientSaveData data)
    {
        bool isDataSaved = false;
        int clientSaveSlot = 0;

        if (data != null)
        {
            foreach (ClientSaveData currentSave in selectedSave.clientSaveDataList)
            {
                if (currentSave.playerProfileData.steamID == data.playerProfileData.steamID)
                {
                    isDataSaved = true;
                    break;
                }
                clientSaveSlot++;
            }

            if (!isDataSaved)
            {
                selectedSave.clientSaveDataList.Add(data);
            }
            else
            {
                Debug.Log("Replacing player " + selectedSave.clientSaveDataList[clientSaveSlot].playerProfileData.playerName + "with player name " + data.playerProfileData.playerName);
                selectedSave.clientSaveDataList[clientSaveSlot] = data;
            }
        }
        else
        {
            Debug.Assert(false);
        }
    }
}
