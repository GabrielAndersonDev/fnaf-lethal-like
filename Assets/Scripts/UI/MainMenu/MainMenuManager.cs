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
    public static MainMenuManager Singleton { get; private set; }

    VisualElement uiDoc;

    [SerializeField]
    SettingsScript settings;

    Box startBox;
    Box saveSelectMenu;

    public Button hostBtn;
    Button clientBtn;
    Button serverBtn;
    Button settingsBtn;
    Button backBtn;

    Button slotZero;
    Button slotZeroDel;

    Button slotOne;
    Button slotOneDel;

    Button slotTwo;
    Button slotTwoDel;

    Button slotThree;
    Button slotThreeDel;

    Box popupBox;
    Box popupOverlay;

    Button popupConfirmBtn;
    Button popupCancelBtn;
    Button popupOkayBtn;

    int selectedSlot = -1;

    bool isHostClicked = false;
    bool isPopup = false;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        if (GameManager.Singleton != null)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            DontDestroyOnLoad(NetworkScript.Singleton);
            uiDoc = GetComponent<UIDocument>().rootVisualElement;

            startBox = uiDoc.Q<Box>("start-box");

            hostBtn = uiDoc.Q<Button>("host-btn");
            clientBtn = uiDoc.Q<Button>("client-btn");
            serverBtn = uiDoc.Q<Button>("server-btn");
            settingsBtn = uiDoc.Q<Button>("settings-btn");

            saveSelectMenu = uiDoc.Q<Box>("save-select-menu");

            slotZero = uiDoc.Q<Button>("slot-0");
            slotZeroDel = slotZero.Q<Button>("delete-btn");

            slotOne = uiDoc.Q<Button>("slot-1");
            slotOneDel = slotOne.Q<Button>("delete-btn");

            slotTwo = uiDoc.Q<Button>("slot-2");
            slotTwoDel = slotTwo.Q<Button>("delete-btn");

            slotThree = uiDoc.Q<Button>("slot-3");
            slotThreeDel = slotThree.Q<Button>("delete-btn");

            backBtn = uiDoc.Q<Button>("back-btn");

            popupBox = uiDoc.Q<Box>("popup-box");
            popupOverlay = uiDoc.Q<Box>("popup-overlay");
            popupConfirmBtn = uiDoc.Q<Button>("popup-confirm-btn");
            popupCancelBtn = uiDoc.Q<Button>("popup-cancel-btn");
            popupOkayBtn = uiDoc.Q<Button>("popup-okay-btn");

            CheckHostState();
            PopupClassCheck();

            RetrieveSaveData(slotZero, 0);
            RetrieveSaveData(slotOne, 1);
            RetrieveSaveData(slotTwo, 2);
            RetrieveSaveData(slotThree, 3);

            hostBtn.clicked += OnHostClicked;
            clientBtn.clicked += OnClientClicked;
            serverBtn.clicked += OnServerClicked;
            settingsBtn.clicked += OnSettingsClicked;

            backBtn.clicked += BackClicked;

            slotZero.clicked += OnSlotZeroClicked;
            slotZeroDel.clicked += OnSlotZeroDelClicked;

            slotOne.clicked += OnSlotOneClicked;
            slotOneDel.clicked += OnSlotOneDelClicked;

            slotTwo.clicked += OnSlotTwoClicked;
            slotTwoDel.clicked += OnSlotTwoDelClicked;

            slotThree.clicked += OnSlotThreeClicked;
            slotThreeDel.clicked += OnSlotThreeDelClicked;
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
            button.Q<TextElement>("slot-title").text = "Slot " + slot;
            button.Q<TextElement>("day").text = "Day: " + data.day.ToString();
            button.Q<TextElement>("money").text = "Money: " + data.money.ToString();
        }
        else
        {
            // change around if i decide on a specific style/design for empty slots
            button.Q<TextElement>("slot-title").text = "Empty";
            button.Q<TextElement>("day").text = "Day: 0";
            button.Q<TextElement>("money").text = "Money: 0";
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
            saveSelectMenu.style.display = DisplayStyle.Flex;
        }
        else
        {
            startBox.style.display = DisplayStyle.Flex;
            saveSelectMenu.style.display = DisplayStyle.None;
        }
    }

    void OnSettingsClicked()
    {
        settings.InitSettingsUI();
    }

    void OnSlotZeroClicked()
    {
        OnSlotClicked(0);
    }

    void OnSlotZeroDelClicked()
    {
        bool isEmpty = GameManager.Singleton.saveDataArray.gameStateArray[0].isEmpty;
        selectedSlot = 0;
        TogglePopup(isEmpty);
    }

    void OnSlotOneClicked()
    {
        OnSlotClicked(1);
    }

    void OnSlotOneDelClicked()
    {
        bool isEmpty = GameManager.Singleton.saveDataArray.gameStateArray[1].isEmpty;
        selectedSlot = 1;
        TogglePopup(isEmpty);
    }

    void OnSlotTwoClicked()
    {
        OnSlotClicked(2);
    }

    void OnSlotTwoDelClicked()
    {
        bool isEmpty = GameManager.Singleton.saveDataArray.gameStateArray[2].isEmpty;
        selectedSlot = 2;
        TogglePopup(isEmpty);
    }

    void OnSlotThreeClicked()
    {
        OnSlotClicked(3);
    }

    void OnSlotThreeDelClicked()
    {
        bool isEmpty = GameManager.Singleton.saveDataArray.gameStateArray[3].isEmpty;
        selectedSlot = 3;
        TogglePopup(isEmpty);
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

    private void PopupClassCheck()
    {
        if (isPopup)
        {
            hostBtn.SetEnabled(false);
            clientBtn.SetEnabled(false);
            serverBtn.SetEnabled(false);
            settingsBtn.SetEnabled(false);
            slotZero.SetEnabled(false);
            slotZeroDel.SetEnabled(false);
            slotOne.SetEnabled(false);
            slotOneDel.SetEnabled(false);
            slotTwo.SetEnabled(false);
            slotTwoDel.SetEnabled(false);
            slotThree.SetEnabled(false);
            slotThreeDel.SetEnabled(false);
            backBtn.SetEnabled(false);

            popupConfirmBtn.SetEnabled(true);
            popupCancelBtn.SetEnabled(true);
            popupOkayBtn.SetEnabled(true);

            popupOverlay.RemoveFromClassList("popup-disabled");
            popupOverlay.AddToClassList("popup-enabled");

            popupBox.RemoveFromClassList("popup-disabled");
            popupBox.AddToClassList("popup-enabled");
        }
        else
        {
            hostBtn.SetEnabled(true);
            clientBtn.SetEnabled(true);
            serverBtn.SetEnabled(true);
            settingsBtn.SetEnabled(true);
            slotZero.SetEnabled(true);
            slotZeroDel.SetEnabled(true);
            slotOne.SetEnabled(true);
            slotOneDel.SetEnabled(true);
            slotTwo.SetEnabled(true);
            slotTwoDel.SetEnabled(true);
            slotThree.SetEnabled(true);
            slotThreeDel.SetEnabled(true);
            backBtn.SetEnabled(true);

            popupConfirmBtn.SetEnabled(false);
            popupCancelBtn.SetEnabled(false);
            popupOkayBtn.SetEnabled(false);

            popupOverlay.RemoveFromClassList("popup-enabled");
            popupOverlay.AddToClassList("popup-disabled");

            popupBox.RemoveFromClassList("popup-enabled");
            popupBox.AddToClassList("popup-disabled");
        }
    }

    private void TogglePopup(bool isEmpty)
    {
        isPopup = !isPopup;
        PopupClassCheck();

        if (isPopup)
        {
            if (isEmpty)
            {
                popupBox.Q<TextElement>("popup-title").text = "This save slot is already empty.";

                popupConfirmBtn.RemoveFromClassList("enabled");
                popupConfirmBtn.AddToClassList("disabled");

                popupCancelBtn.RemoveFromClassList("enabled");
                popupCancelBtn.AddToClassList("disabled");

                popupOkayBtn.RemoveFromClassList("disabled");
                popupOkayBtn.AddToClassList("enabled");

                popupOkayBtn.clicked += PopupCancelBtnClicked;
            }
            else
            {
                popupBox.Q<TextElement>("popup-title").text = "Are you sure you would like to delete this save file?";

                popupConfirmBtn.RemoveFromClassList("disabled");
                popupConfirmBtn.AddToClassList("enabled");

                popupCancelBtn.RemoveFromClassList("disabled");
                popupCancelBtn.AddToClassList("enabled");

                popupOkayBtn.RemoveFromClassList("enabled");
                popupOkayBtn.AddToClassList("disabled");

                popupConfirmBtn.clicked += PopupConfirmBtnClicked;
                popupCancelBtn.clicked += PopupCancelBtnClicked;
            }
        }
        else
        {
            popupConfirmBtn.clicked -= PopupConfirmBtnClicked;
            popupCancelBtn.clicked -= PopupCancelBtnClicked;
            popupOkayBtn.clicked -= PopupCancelBtnClicked;
        }
    }

    void PopupConfirmBtnClicked()
    {
        switch (selectedSlot)
        {
            case 0:
                SaveManager.DeleteSaveSlot(0);
                RetrieveSaveData(slotZero, 0);
                break;
            case 1:
                SaveManager.DeleteSaveSlot(1);
                RetrieveSaveData(slotOne, 1);
                break;
            case 2:
                SaveManager.DeleteSaveSlot(2);
                RetrieveSaveData(slotTwo, 2);
                break;
            case 3:
                SaveManager.DeleteSaveSlot(3);
                RetrieveSaveData(slotThree, 3);
                break;
            default:
                Debug.LogError("PopupConfirmBtnClicked: selectedSlot invalid value");
                break;
        }

        TogglePopup(false);
        selectedSlot = -1;
    }

    void PopupCancelBtnClicked()
    {
        TogglePopup(false);
        selectedSlot = -1;
    }

    private void OnDestroy()
    {
        hostBtn.clicked -= OnHostClicked;
        clientBtn.clicked -= OnClientClicked;
        serverBtn.clicked -= OnServerClicked;
        settingsBtn.clicked -= OnSettingsClicked;

        backBtn.clicked -= BackClicked;

        slotZero.clicked -= OnSlotZeroClicked;
        slotZeroDel.clicked -= OnSlotZeroDelClicked;

        slotOne.clicked -= OnSlotOneClicked;
        slotOneDel.clicked -= OnSlotOneDelClicked;

        slotTwo.clicked -= OnSlotTwoClicked;
        slotTwoDel.clicked -= OnSlotTwoDelClicked;

        slotThree.clicked -= OnSlotThreeClicked;
        slotThreeDel.clicked -= OnSlotThreeDelClicked;
    }
}
