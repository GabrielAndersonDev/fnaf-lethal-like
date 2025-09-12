using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        SceneManager.LoadScene("MainMenu");
    }
}