using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject networkManager;

    NetworkScript netScript;
    int maxSegInt = 20;
    VisualElement uiDoc;
    bool useRandomSeed = true;

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
        maxSegInt = integerField.value;
        GameManager.Instance.gameData.maxSegmentCount = maxSegInt;

        if (!useRandomSeed)
        {
            IntegerField seedField = uiDoc.Q<IntegerField>("seed");
            GameManager.Instance.seed.Value = seedField.value;
            GameManager.Instance.useRandomSeed.Value = true;
        }
        else
        {
            GameManager.Instance.seed.Value = 0;
            GameManager.Instance.useRandomSeed.Value = false;
        }
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
}
