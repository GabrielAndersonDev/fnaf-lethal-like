using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public static Game main;
    public GameObject itemManagerObject;
    public GameObject mapManagerObject;
    public GameObject playerManagerObject;

    public PlayerData basePlayerData;

    ItemManager itemManager;
    MapManager mapManager;
    PlayerManager playerManager;

    Quaternion quaternion;

    void Awake()
    {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        itemManager = itemManagerObject.GetComponent<ItemManager>();
        mapManager = mapManagerObject.GetComponent<MapManager>();
        playerManager = playerManagerObject.GetComponent<PlayerManager>();

        quaternion = Quaternion.identity;

        playerManager.SpawnPlayer(basePlayerData, this.transform.position, quaternion);
    }

    //void StartTestGame()
    //{
        
    //}

    // Update is called once per frame
    //public void Pause()
    //{
        
    //}
}
