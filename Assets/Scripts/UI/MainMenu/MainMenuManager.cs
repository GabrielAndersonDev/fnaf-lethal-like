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

            hostBtn.clicked += OnHostClicked;
            clientBtn.clicked += OnClientClicked;
            serverBtn.clicked += OnServerClicked;
        }
        else
        {
            Debug.LogError("MainMenuManager: GameManager not found");
            Debug.Break();
        }
    }

    public void OnHostClicked()
    {
        IntegerField integerField = uiDoc.Q<IntegerField>("max-seg-int");
        maxSegInt = integerField.value;
        GameManager.Instance.gameData.maxSegmentCount = maxSegInt;

        netScript.LoadHostGame();
    }

    public void OnClientClicked()
    {
        netScript.LoadClient();
    }

    public void OnServerClicked()
    {
        netScript.LoadServer();
    }
}
