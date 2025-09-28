using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton { get; private set; }

    public NetworkVariable<SerializableGameInfo> gameInfo = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> sessionSeed = new(writePerm: NetworkVariableWritePermission.Server);

    // the day will help determine future difficulty values with scaling
    public NetworkVariable<int> day = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> money = new(writePerm: NetworkVariableWritePermission.Server);

    public GameStateData selectedSave;
    public SaveDataArray saveDataArray = new();

    [HideInInspector]
    public SavedPlayerSettings playerSettings;

    public SavedPlayerSettings defaultPlayerSettings;

    void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
        InitializeGame();
    }

    void InitializeGame()
    {
        // use for save data + other stuff we need to maintain
        SaveManager.LoadSaves();
        SceneManager.LoadScene("MainMenu");
    }

    void GenerateNewGameInfo()
    {
        // will add more function here later, for now will just generate a new seed wwith the same base stats.
        if (!NetworkManager.Singleton.IsHost
            || !NetworkManager.Singleton.IsServer)
        {
            UnityEngine.Debug.Log("Clients can't use this");
            return;
        }

        day.Value = ++day.Value;
        UnityEngine.Debug.Log("Day is: " + day.Value);

        int seed = UnityEngine.Random.Range(0, 999999);
        UnityEngine.Debug.Log("New seed is: " + seed);

        GameInfo info = ScriptableObject.CreateInstance<GameInfo>();
        info.GetGameInfoFromSerialized(info, gameInfo.Value);
        info.Seed = seed;
        gameInfo.Value = info.GetSerializableGameInfo();
    }

    [ServerRpc]
    public void GenerateNewGameInfoServerRpc()
    {
        GenerateNewGameInfo();
    }

    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton != null)
        {
            if (SceneManager.GetActiveScene().name == "VanScene"
            && NetworkManager.Singleton.IsHost)
            {
                SaveManager.SaveGameData();
            }

            if (NetworkManager.Singleton.IsClient
                && !NetworkManager.Singleton.IsHost)
            {
                ClientSaveData data = SaveManager.SaveLocalClientData();
                SaveManager.SendClientSaveDataServerRpc(data.GetSerializedClientData());
            }
        }
    }
}