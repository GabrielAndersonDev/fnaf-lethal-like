using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.TextCore;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject networkManager;

    NetworkScript netScript;
    VisualElement uiDoc;

    // these are here until i impliment a better way to determine these before adding them
    int seed;
    bool useRandomSeed = true;
    int maxSegCount = 20;
    int roomCount = 6;
    int staffMin = 1;
    int bathMin = 1;
    float diffSegBoost = 2;

    

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            DontDestroyOnLoad(networkManager);
            uiDoc = GetComponent<UIDocument>().rootVisualElement;
            netScript = networkManager.GetComponent<NetworkScript>();

            Button hostBtn = uiDoc.Q<Button>("host-btn");
            Button clientBtn = uiDoc.Q<Button>("client-btn");
            Button serverBtn = uiDoc.Q<Button>("server-btn");
            Button useRandBtn = uiDoc.Q<Button>("use-rand-btn");

            useRandBtn.text = useRandomSeed ? "True" : "False";

            hostBtn.clicked += OnHostClicked;
            clientBtn.clicked += OnClientClicked;
            serverBtn.clicked += OnServerClicked;
            useRandBtn.clicked += OnRandClicked;
        }
        else
        {
            Debug.LogError("MainMenuManager: GameManager not found");
            Debug.Break();
        }
    }

    public void OnHostClicked()
    {
        netScript.LoadHostGame();

        IntegerField integerField = uiDoc.Q<IntegerField>("max-seg-int");
        maxSegCount = integerField.value;

        if (!useRandomSeed)
        {
            IntegerField seedField = uiDoc.Q<IntegerField>("seed");
            seed = seedField.value;
        }
        else
        {
            seed = Random.Range(0, 999999);
        }

        GameManager.Instance.gameInfo.Value = CreateNewGameInfo();
    }

    public void OnClientClicked()
    {
        netScript.LoadClient();
    }

    public void OnServerClicked()
    {
        netScript.LoadServer();
    }

    public void OnRandClicked()
    {
        Button useRandomBtn = uiDoc.Q<Button>("use-rand-btn");
        useRandomSeed = !useRandomSeed;
        useRandomBtn.text = useRandomSeed ? "True" : "False";
    }

    public GameInfo CreateNewGameInfo()
    {
        GameInfo gameInfo = new()
        {
            Seed = seed,
            UseRandSeed = useRandomSeed,
            MaxSegmentCount = maxSegCount,
            RoomCount = roomCount,
            StaffMin = staffMin,
            BathMin = bathMin,
            DiffSegBoost = diffSegBoost
        };

        return gameInfo;
    }
}
