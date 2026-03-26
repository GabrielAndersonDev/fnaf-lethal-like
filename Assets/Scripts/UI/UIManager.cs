using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem.UI;
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

    [SerializeField]
    SettingsScript settings;

    public InputSystemUIInputModule InputModule;

    Player player;

    Box[] inventorySlots;

    TextElement playerName;
    TextElement playerHealth;
    TextElement playerStamina;

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
        else
        {
            Singleton = this;
        }

        DontDestroyOnLoad(gameObject);
        InitUI();
    }

    public void AssignPlayerToUI(Player player)
    {
        this.player = player;

        playerName = GUI.Q<TextElement>("player-name");
        playerName.text = player.playerName.ToString();

        playerHealth = GUI.Q<TextElement>("health-amount");
        playerHealth.text = player.currentHealth.ToString() + "/" + player.baseHealth.ToString();
        playerStamina = GUI.Q<TextElement>("stamina-amount");
        playerStamina.text = player.baseStamina.ToString() + "/" + player.baseStamina.ToString();

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
    }

    public void InitUI()
    {
        pauseUi = pauseObject.GetComponent<UIDocument>().rootVisualElement;
        GUI = guiObject.GetComponent<UIDocument>().rootVisualElement;

        // Get player reassigns itself to the player upon death if the model changes (aka, following another player around)

        resumeBtn = pauseUi.Q<Button>("resume-btn");
        settingsBtn = pauseUi.Q<Button>("settings-btn");
        mainReturnBtn = pauseUi.Q<Button>("main-return-btn");
        quitBtn = pauseUi.Q<Button>("quit-btn");

        popupOverlay = pauseUi.Q<Box>("popup-overlay");
        popupBox = pauseUi.Q<Box>("popup-box");
        popupTitle = pauseUi.Q<TextElement>("popup-title");
        popupConfirmBtn = pauseUi.Q<Button>("popup-confirm-btn");
        popupCancelBtn = pauseUi.Q<Button>("popup-cancel-btn");
    }

    private void OnEnable()
    {
        resumeBtn.clicked += ResumeBtnClicked;
        settingsBtn.clicked += SettingsBtnClicked;
        mainReturnBtn.clicked += MainReturnBtnClicked;
        quitBtn.clicked += QuitBtnClicked;
    }

    private void OnDisable()
    {
        resumeBtn.clicked -= ResumeBtnClicked;
        settingsBtn.clicked -= SettingsBtnClicked;
        mainReturnBtn.clicked -= MainReturnBtnClicked;
        quitBtn.clicked -= QuitBtnClicked;
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
        Debug.Log(player.playerActionMap.FindAction("Move") + " is enabled in toggle pause? " + player.playerActionMap.FindAction("Move").enabled);

        if (!player.isPaused)
        {
            Debug.Log("unpausing game...");
            pauseUi.SetEnabled(false);
            GUI.SetEnabled(true);
            player.isPaused = false;
            pauseUi.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            player.SetPlayerInputMap(true);
        }
        else
        {
            Debug.Log("pausing game...");
            pauseUi.SetEnabled(true);
            GUI.SetEnabled(false);
            player.isPaused = true;
            pauseUi.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            player.SetPlayerInputMap(false);
        }
    }

    private void ResumeBtnClicked()
    {
        Debug.Log("Resume pressed");
        TogglePause();
    }

    private void SettingsBtnClicked()
    {
        pauseUi.style.display = pauseUi.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        settings.InitSettingsUI();
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

        if (SceneManager.GetActiveScene().name == "VanScene"
            && NetworkManager.Singleton.IsHost)
        {
            SaveManager.SaveGameData();
        }

        NetworkScript.Singleton.Disconnect();

        // True returns to main menu, false quits game
        if (isReturnMain)
        {
            Debug.Log("Returning to main menu...");
            NetworkScript.Singleton.LoadMainMenu();
            Destroy(gameObject);
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
