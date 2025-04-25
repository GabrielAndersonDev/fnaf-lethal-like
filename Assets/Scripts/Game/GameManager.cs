using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameData gameData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGame();
    }

    void InitializeGame()
    {
        // use for save data + other stuff we need to maintain
        SceneManager.LoadScene("MainMenu");
    }

    //void Start()
    //{
    //    mapManager = mapManagerObject.GetComponent<MapManager>();
    //    mapManager.difficulty.maxSegmentCount = gameData.segmentCountInt;
    //    mapManager.MapManagerInit();

    //    itemManager = itemManagerObject.GetComponent<ItemManager>();
    //    itemManager.ItemManagerInit();

    //    playerManager = playerManagerObject.GetComponent<PlayerManager>();

    //    quaternion = Quaternion.identity;

    //    playerManager.SpawnPlayer(playerManager.basePlayerData, this.transform.position, quaternion);
    //}

    //void StartTestGame()
    //{
        
    //}
}
