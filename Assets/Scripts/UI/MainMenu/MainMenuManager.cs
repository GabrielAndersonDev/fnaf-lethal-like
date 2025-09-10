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

    Box startBox;
    Box saveSlotContainer;

    bool isHostClicked = false;
    
    private void Start()
    {
        if (GameManager.Singleton != null)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            DontDestroyOnLoad(NetworkScript.Singleton);
            uiDoc = GetComponent<UIDocument>().rootVisualElement;

            startBox = uiDoc.Q<Box>("start-box");

            Button hostBtn = uiDoc.Q<Button>("host-btn");
            Button clientBtn = uiDoc.Q<Button>("client-btn");
            Button serverBtn = uiDoc.Q<Button>("server-btn");

            saveSlotContainer = uiDoc.Q<Box>("save-slot-container");

            Button slotZero = uiDoc.Q<Button>("slot-0");
            Button slotOne = uiDoc.Q<Button>("slot-1");
            Button slotTwo = uiDoc.Q<Button>("slot-2");
            Button slotThree = uiDoc.Q<Button>("slot-3");

            Button backBtn = uiDoc.Q<Button>("back-btn");

            if (!isHostClicked)
            {
                startBox.style.display = DisplayStyle.Flex;
                saveSlotContainer.style.display = DisplayStyle.None;
            }
            else
            {
                startBox.style.display = DisplayStyle.None;
                saveSlotContainer.style.display = DisplayStyle.Flex;
            }

            hostBtn.clicked += OnHostClicked;
            clientBtn.clicked += OnClientClicked;
            serverBtn.clicked += OnServerClicked;

            slotZero.clicked += OnSlotClicked(0);
        }
        else
        {
            Debug.LogError("MainMenuManager: GameManager not found");
            Debug.Break();
        }
    }

    public void OnHostClicked()
    {
        isHostClicked = true;

        if (!isHostClicked)
        {
            startBox.style.display = DisplayStyle.Flex;
            saveSlotContainer.style.display = DisplayStyle.None;
        }
        else
        {
            startBox.style.display = DisplayStyle.None;
            saveSlotContainer.style.display = DisplayStyle.Flex;
        }
    }

    void OnSlotClicked(int slot)
    {
        NetworkScript.Singleton.LoadHostGame();
    }

    public void OnClientClicked()
    {
        NetworkScript.Singleton.LoadClient();
    }

    public void OnServerClicked()
    {
        NetworkScript.Singleton.LoadServer();
        NetworkManager.Singleton.SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
    }
}
