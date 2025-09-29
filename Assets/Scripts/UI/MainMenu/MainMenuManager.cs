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
            Button settingsBtn = uiDoc.Q<Button>("settings-btn");

            saveSlotContainer = uiDoc.Q<Box>("save-slot-container");

            Button slotZero = uiDoc.Q<Button>("slot-0");
            Button slotOne = uiDoc.Q<Button>("slot-1");
            Button slotTwo = uiDoc.Q<Button>("slot-2");
            Button slotThree = uiDoc.Q<Button>("slot-3");

            Button backBtn = uiDoc.Q<Button>("back-btn");

            CheckHostState();

            RetrieveSaveData(slotZero, 0);
            RetrieveSaveData(slotOne, 1);
            RetrieveSaveData(slotTwo, 2);
            RetrieveSaveData(slotThree, 3);

            hostBtn.clicked += OnHostClicked;
            clientBtn.clicked += OnClientClicked;
            serverBtn.clicked += OnServerClicked;

            backBtn.clicked += BackClicked;

            slotZero.clicked += OnSlotZeroClicked;
            slotOne.clicked += OnSlotOneClicked;
            slotTwo.clicked += OnSlotTwoClicked;
            slotThree.clicked += OnSlotThreeClicked;
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

        CheckHostState();
    }

    void RetrieveSaveData(Button button, int slot)
    {
        GameStateData data = GameManager.Singleton.saveDataArray.gameStateArray[slot];

        if (!data.isEmpty)
        {
            button.Q<TextElement>("slot-title").text = "Saved Game: Slot " + slot;
            button.Q<TextElement>("day").text = data.day.ToString();
            button.Q<TextElement>("money").text = data.money.ToString();
        }
        else
        {
            // change around if i decide on a specific style/design for empty slots
            button.Q<TextElement>("slot-title").text = "Empty";
            button.Q<TextElement>("day").text = "0";
            button.Q<TextElement>("money").text = "0";
        }
    }

    void BackClicked()
    {
        isHostClicked = false;

        CheckHostState();
    }

    void CheckHostState()
    {
        if (isHostClicked)
        {
            startBox.style.display = DisplayStyle.None;
            saveSlotContainer.style.display = DisplayStyle.Flex;
        }
        else
        {
            startBox.style.display = DisplayStyle.Flex;
            saveSlotContainer.style.display = DisplayStyle.None;
        }
    }

    void OnSlotZeroClicked()
    {
        OnSlotClicked(0);
    }

    void OnSlotOneClicked()
    {
        OnSlotClicked(1);
    }

    void OnSlotTwoClicked()
    {
        OnSlotClicked(2);
    }

    void OnSlotThreeClicked()
    {
        OnSlotClicked(3);
    }

    void OnSlotClicked(int slot)
    {
        SaveManager.SelectSaveSlot(slot);

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
