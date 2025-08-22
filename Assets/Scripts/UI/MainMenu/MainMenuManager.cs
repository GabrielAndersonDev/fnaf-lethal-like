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
    VisualElement uiDoc;
    
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            DontDestroyOnLoad(NetworkScript.Singleton);
            uiDoc = GetComponent<UIDocument>().rootVisualElement;

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
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
    }

    public void OnClientClicked()
    {
        NetworkScript.Singleton.LoadClient();
    }

    public void OnServerClicked()
    {
        NetworkScript.Singleton.LoadServer();
    }
}
