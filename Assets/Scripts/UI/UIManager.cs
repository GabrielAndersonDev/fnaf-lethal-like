using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseObject;
    VisualElement pauseUi;
    [SerializeField]
    GameObject guiObject;
    VisualElement GUI;

    Player player;

    Box[] inventorySlots;

    Button resumeBtn;
    Button settingsBtn;
    Button mainReturnBtn;
    Button quitBtn;
    Box popupOverlay;
    Box popupBox;
    TextElement popupTitle;
    Button popupConfirmBtn;
    Button popupCancelBtn;

    public static UIManager Singleton { get; private set; }
    
    bool isPopup = false;
    private bool isReturnMain;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }

    public void InitUI(Player p)
    {
        pauseUi = pauseObject.GetComponent<UIDocument>().rootVisualElement;
        GUI = guiObject.GetComponent<UIDocument>().rootVisualElement;

        // Get player reassigns itself to the player upon death if the model changes (aka, following another player around)

        player = p;

        resumeBtn = pauseUi.Q<Button>("resume-btn");
        settingsBtn = pauseUi.Q<Button>("settings-btn");
        mainReturnBtn = pauseUi.Q<Button>("main-return-btn");
        quitBtn = pauseUi.Q<Button>("quit-btn");

        popupOverlay = pauseUi.Q<Box>("popup-overlay");
        popupBox = pauseUi.Q<Box>("popup-box");
        popupTitle = pauseUi.Q<TextElement>("popup-title");
        popupConfirmBtn = pauseUi.Q<Button>("popup-confirm-btn");
        popupCancelBtn = pauseUi.Q<Button>("popup-cancel-btn");

        InitInventorySlots();

        if (player.isPaused)
        {
            pauseUi.SetEnabled(true);
            GUI.SetEnabled(false);
            pauseUi.style.display = DisplayStyle.Flex;
        }
        else
        {
            pauseUi.SetEnabled(false);
            GUI.SetEnabled(true);
            pauseUi.style.display = DisplayStyle.None;
        }

        PopupClassCheck();

        resumeBtn.clicked += ResumeBtnClicked;
        settingsBtn.clicked += SettingsBtnClicked;
        mainReturnBtn.clicked += MainReturnBtnClicked;
        quitBtn.clicked += QuitBtnClicked;
    }

    private void InitInventorySlots()
    {
        List<Box> slots = GUI.Query<Box>(className: "item-slot").ToList();
        inventorySlots = new Box[slots.Count];

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (slots[i] != null)
            {
                inventorySlots[i] = slots[i];
            }
            else
            {
                Debug.LogError($"{slots[i]} is null");
                Debug.Break();
            }
        }
    }

    private void PopupClassCheck()
    {
        if (isPopup)
        {
            GUI.SetEnabled(false);
            resumeBtn.SetEnabled(false);
            settingsBtn.SetEnabled(false);
            mainReturnBtn.SetEnabled(false);
            quitBtn.SetEnabled(false);

            popupOverlay.RemoveFromClassList("popup-disabled");
            popupOverlay.AddToClassList("popup-enabled");

            popupBox.RemoveFromClassList("popup-disabled");
            popupBox.AddToClassList("popup-enabled");
        }
        else
        {
            GUI.SetEnabled(true);
            resumeBtn.SetEnabled(true);
            settingsBtn.SetEnabled(true);
            mainReturnBtn.SetEnabled(true);
            quitBtn.SetEnabled(true);

            popupOverlay.RemoveFromClassList("popup-enabled");
            popupOverlay.AddToClassList("popup-disabled");

            popupBox.RemoveFromClassList("popup-enabled");
            popupBox.AddToClassList("popup-disabled");
        }
    }
    
    public void TogglePause()
    {
        player.isPaused = !player.isPaused;

        if (!player.isPaused)
        {
            pauseUi.SetEnabled(false);
            GUI.SetEnabled(true);
            player.isPaused = false;
            pauseUi.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

        }
        else
        {
            pauseUi.SetEnabled(true);
            GUI.SetEnabled(false);
            player.isPaused = true;
            pauseUi.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }

    private void ResumeBtnClicked()
    {
        TogglePause();
    }

    private void SettingsBtnClicked()
    {
        Debug.LogError("Settings doesn't exist yet :(");
        Debug.Break();
    }

    private void MainReturnBtnClicked()
    {
        isReturnMain = true;
        TogglePopup("Are you sure you'd like to return to the main menu?");
    }

    private void QuitBtnClicked()
    {
        isReturnMain = false;
        TogglePopup("Are you sure you'd like to quit?");
    }

    private void TogglePopup(string textEntry)
    {
        isPopup = !isPopup;
        PopupClassCheck();

        if (isPopup)
        {
            popupTitle.text = textEntry;

            popupConfirmBtn.clicked += PopupConfirmBtnClicked;
            popupCancelBtn.clicked += PopupCancelBtnClicked;
        }
        else
        {
            popupConfirmBtn.clicked -= PopupConfirmBtnClicked;
            popupCancelBtn.clicked -= PopupCancelBtnClicked;
        }
    }

    private void PopupConfirmBtnClicked()
    {
        Debug.Log("Stopping host or server...");
        NetworkScript.Singleton.Disconnect();

        // True returns to main menu, false quits game
        if (isReturnMain)
        {
            Debug.Log("Returning to main menu...");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        }
    }

    private void PopupCancelBtnClicked()
    {
        TogglePopup("none");
    }

    public void InventoryUIUpdate(int slot)
    {
        Image img = inventorySlots[slot].Q<Image>(className: "sprite-slot");
        Label label = inventorySlots[slot].Q<Label>(className: "label-slot");

        if (player.inventory[slot] != null)
        {
            img.sprite = player.inventory[slot].icon;
            label.text = player.inventory[slot].itemName;
        }
        else
        {
            img.sprite = null;
            label.text = "Empty";
        }
    }
}
