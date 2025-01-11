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
    public GameObject itemManagerPrefab;
    public GameObject mapManagerPrefab;

    MapManager mapManager;

    //public ItemManager itemManager;

    void Awake()
    {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        CreateMapManager();
        CreateItemManager();
    }

    private void CreateItemManager()
    {
        _ = Instantiate(itemManagerPrefab);
    }

    private void CreateMapManager()
    {
        _ = Instantiate(mapManagerPrefab);
        mapManager = GameObject.FindObjectOfType<MapManager>();
    }
    //void StartTestGame()
    //{
        
    //}

    // Update is called once per frame
    //public void Pause()
    //{
        
    //}
}
