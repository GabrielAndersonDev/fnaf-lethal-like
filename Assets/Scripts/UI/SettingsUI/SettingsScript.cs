using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingsScript : MonoBehaviour
{
    [SerializeField]
    GameObject settingsObj;
    VisualElement settingsUI;

    [SerializeField]
    VisualTreeAsset keyTemplate;

    Box settingsBox;
    Box keySettingsBox;
    Box popupBox;

    Button keySettingsBtn;
    Toggle playerListToggle;
    Toggle sprintToggle;
    Toggle crouchToggle;
    Button applyBtn;
    Button resetAllPriorBtn;
    Button resetAllDefaultBtn;
    Button backBtn;
    Box popupOverlay;

    VisualElement keyBindingsContainer;
    TextField keySearch;
    ScrollView keyBindingsScroll;
    Button resetToPriorKeysBtn;
    Button resetToDefaultKeysBtn;
    Button backFromKeySettingsBtn;

    Button popupConfirmBtn;
    Button popupCancelBtn;
    Button popupBackBtn;

    bool isChanged = false;
    bool isPopup = false;

    private void Awake()
    {
        settingsUI = settingsObj.GetComponent<UIDocument>().rootVisualElement;

        settingsBox = settingsUI.Q<Box>("settings-box");
        keySettingsBox = settingsUI.Q<Box>("key-settings-container");
        popupBox = settingsUI.Q<Box>("popup-box");

        keySettingsBtn = settingsBox.Q<Button>("key-settings-btn");
        playerListToggle = settingsBox.Q<Toggle>("player-list-toggle");
        sprintToggle = settingsBox.Q<Toggle>("sprint-toggle");
        crouchToggle = settingsBox.Q<Toggle>("crouch-toggle");
        applyBtn = settingsBox.Q<Button>("apply-btn");
        resetAllPriorBtn = settingsBox.Q<Button>("reset-all-prior-btn");
        resetAllDefaultBtn = settingsBox.Q<Button>("reset-all-default-btn");
        backBtn = settingsBox.Q<Button>("back-btn");
        popupOverlay = settingsBox.Q<Box>("popup-overlay");

        keyBindingsContainer = keySettingsBox.Q<VisualElement>("key-bindings-container");
        keySearch = keySettingsBox.Q<TextField>("key-search");
        keyBindingsScroll = keySettingsBox.Q<ScrollView>("key-bindings-scrollview");
        resetToPriorKeysBtn = keySettingsBox.Q<Button>("reset-keys-prior-btn");
        resetToDefaultKeysBtn = keySettingsBox.Q<Button>("reset-keys-default-btn");
        backFromKeySettingsBtn = keySettingsBox.Q<Button>("back-from-key-settings-btn");

        popupConfirmBtn = popupBox.Q<Button>("popup-confirm-btn");
        popupCancelBtn = popupBox.Q<Button>("popup-cancel-btn");
        popupBackBtn = popupBox.Q<Button>("popup-back-btn");

        settingsUI.style.display = DisplayStyle.None;
    }

    public void InitSettingsUI()
    {
        settingsUI.style.display = DisplayStyle.Flex;

        keySettingsBtn.clicked += OpenKeySettings;
        applyBtn.clicked += ApplySettings;
        resetAllPriorBtn.clicked += ResetAllToPrior;
        resetAllDefaultBtn.clicked += ResetAllToDefault;
        backBtn.clicked += CloseSettings;
        resetToDefaultKeysBtn.clicked += ResetToDefaultKeys;
        backFromKeySettingsBtn.clicked += CloseKeySettings;

        playerListToggle.value = GameManager.Singleton.playerSettings.isTogglePlayerList;
        sprintToggle.value = GameManager.Singleton.playerSettings.isToggleSprint;
        crouchToggle.value = GameManager.Singleton.playerSettings.isToggleCrouch;

        PopupClassCheck();
        PopulateKeyBindings();
    }

    void PopulateKeyBindings()
    {
        if (GameManager.Singleton == null)
        {
            Debug.LogError("GameManager.Singleton is null");
            Debug.Break();
        }

        keyBindingsContainer.Clear();
        foreach (KeyCodeObj keyObj in GameManager.Singleton.playerSettings.keyArray)
        {
            KeyTemplate keyTemplateInstance = keyTemplate.Instantiate().Q<KeyTemplate>();
            keyTemplateInstance.TemplateInit(keyObj);
            keyBindingsContainer.Add(keyTemplateInstance);
        }
    }

    void OpenKeySettings()
    {
        settingsBox.style.display = DisplayStyle.None;
        keySettingsBox.style.display = DisplayStyle.Flex;
    }

    void CloseKeySettings()
    {
        settingsBox.style.display = DisplayStyle.Flex;
        keySettingsBox.style.display = DisplayStyle.None;
    }

    void ResetAllToPrior()
    {
        playerListToggle.value = GameManager.Singleton.playerSettings.isTogglePlayerList;
        sprintToggle.value = GameManager.Singleton.playerSettings.isToggleSprint;
        crouchToggle.value = GameManager.Singleton.playerSettings.isToggleCrouch;

        ResetToPriorKeys();

        isChanged = false;
    }

    void ResetToPriorKeys()
    {
        foreach (VisualElement element in keyBindingsContainer.Children())
        {
            if (element.GetType() == typeof(VisualElement))
            {
                KeyTemplate key = (KeyTemplate)element;

                key.ResetBtnClicked();
            }
        }
    }

    void ResetAllToDefault()
    {
        playerListToggle.value = GameManager.Singleton.defaultPlayerSettings.isTogglePlayerList;
        sprintToggle.value = GameManager.Singleton.defaultPlayerSettings.isToggleSprint;
        crouchToggle.value = GameManager.Singleton.defaultPlayerSettings.isToggleCrouch;

        ResetToDefaultKeys();

        // write smth to compare default and prior settings
    }

    void ResetToDefaultKeys()
    {
        foreach (VisualElement element in keyBindingsContainer.Children())
        {
            if (element.GetType() == typeof(VisualElement))
            {
                KeyTemplate key = (KeyTemplate)element;

                key.ResetDefaultClicked();
            }
        }
    }

    void ApplySettings()
    {
        SavedPlayerSettings settings = GameManager.Singleton.playerSettings;
        KeyCodeObj[] array = settings.keyArray;

        List<KeyCodeObj> keyList = new();
        keyList.Clear();

        foreach (VisualElement element in keyBindingsContainer.Children())
        {
            KeyTemplate key;

            if (element.GetType() == typeof(KeyTemplate))
            {
                key = (KeyTemplate)element;

                if (keyList.Contains(key.keyCodeObj))
                {
                    Debug.LogError("keyCodeObj already exists in keyList");
                    Debug.Break();
                }

                keyList.Add(key.keyCodeObj);
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name != keyList[i].name)
            {
                Debug.LogError("KeyList out of order.");
                Debug.Break();
            }

            array[i]  = keyList[i];
        }

        settings.isTogglePlayerList = playerListToggle.value;
        settings.isToggleSprint = sprintToggle.value;
        settings.isToggleCrouch = crouchToggle.value;

        SaveManager.SaveSettingsToJson();
    }

    public void ToggleAllBtnClickable(KeyTemplate template)
    {
        foreach (VisualElement element in keyBindingsContainer.Children())
        {
            if (element.GetType() == typeof(KeyTemplate))
            {
                KeyTemplate key = (KeyTemplate)element;
                key.ToggleBtnClickable(template);
                return;
            }
        }
    }

    private void PopupClassCheck()
    {
        if (isPopup)
        {
            keySettingsBtn.SetEnabled(false);
            playerListToggle.SetEnabled(false);
            sprintToggle.SetEnabled(false);
            crouchToggle.SetEnabled(false);
            applyBtn.SetEnabled(false);
            resetAllPriorBtn.SetEnabled(false);
            resetAllDefaultBtn.SetEnabled(false);
            backBtn.SetEnabled(false);

            foreach (VisualElement element in keyBindingsContainer.Children())
            {
                element.Q<Button>("key-btn").SetEnabled(false);
                element.Q<Button>("key-reset-btn").SetEnabled(false);
            }

            keySearch.SetEnabled(false);
            keyBindingsScroll.SetEnabled(false);
            resetToPriorKeysBtn.SetEnabled(false);
            resetToDefaultKeysBtn.SetEnabled(false);
            backFromKeySettingsBtn.SetEnabled(false);

            popupConfirmBtn.SetEnabled(true);
            popupCancelBtn.SetEnabled(true);
            popupBackBtn.SetEnabled(true);

            popupOverlay.RemoveFromClassList("popup-disabled");
            popupOverlay.AddToClassList("popup-enabled");

            popupBox.RemoveFromClassList("popup-disabled");
            popupBox.AddToClassList("popup-enabled");
        }
        else
        {
            keySettingsBtn.SetEnabled(true);
            playerListToggle.SetEnabled(true);
            sprintToggle.SetEnabled(true);
            crouchToggle.SetEnabled(true);
            applyBtn.SetEnabled(true);
            resetAllPriorBtn.SetEnabled(true);
            resetAllDefaultBtn.SetEnabled(true);
            backBtn.SetEnabled(true);

            foreach (VisualElement element in keyBindingsContainer.Children())
            {
                element.Q<Button>("key-btn").SetEnabled(true);
                element.Q<Button>("key-reset-btn").SetEnabled(true);
            }

            keySearch.SetEnabled(true);
            keyBindingsScroll.SetEnabled(true);
            resetToPriorKeysBtn.SetEnabled(true);
            resetToDefaultKeysBtn.SetEnabled(true);
            backFromKeySettingsBtn.SetEnabled(true);

            popupConfirmBtn.SetEnabled(false);
            popupCancelBtn.SetEnabled(false);
            popupBackBtn.SetEnabled(false);

            popupOverlay.RemoveFromClassList("popup-enabled");
            popupOverlay.AddToClassList("popup-disabled");

            popupBox.RemoveFromClassList("popup-enabled");
            popupBox.AddToClassList("popup-disabled");
        }
    }

    private void TogglePopup()
    {
        isPopup = !isPopup;
        PopupClassCheck();

        if (isPopup)
        {
            popupConfirmBtn.clicked += PopupConfirmBtnClicked;
            popupCancelBtn.clicked += PopupCancelBtnClicked;
            popupBackBtn.clicked += PopupBackBtnClicked;
        }
        else
        {
            popupConfirmBtn.clicked -= PopupConfirmBtnClicked;
            popupCancelBtn.clicked -= PopupCancelBtnClicked;
            popupBackBtn.clicked -= PopupBackBtnClicked;
        }
    }

    private void PopupConfirmBtnClicked()
    {
        ApplySettings();
        TogglePopup();
        CloseSettings();
    }

    private void PopupCancelBtnClicked()
    {
        TogglePopup();
        CloseSettings();
    }

    private void PopupBackBtnClicked()
    {
        TogglePopup();
    }

    public void ToggleIsChange(bool status)
    {
        isChanged = status;
    }

    public void CloseSettings()
    {
        if (playerListToggle.value != GameManager.Singleton.playerSettings.isTogglePlayerList
            || sprintToggle.value != GameManager.Singleton.playerSettings.isToggleSprint
            || crouchToggle.value != GameManager.Singleton.playerSettings.isToggleCrouch)
        {
            ToggleIsChange(true);
        }

        if (isChanged)
        {
            TogglePopup();
            return;
        }

        if (isPopup)
        {
            TogglePopup();
        }

        keySettingsBtn.clicked -= OpenKeySettings;
        applyBtn.clicked -= ApplySettings;
        resetAllPriorBtn.clicked -= ResetAllToPrior;
        resetAllDefaultBtn.clicked -= ResetAllToDefault;
        backBtn.clicked -= CloseSettings;
        resetToDefaultKeysBtn.clicked -= ResetToDefaultKeys;
        backFromKeySettingsBtn.clicked -= CloseKeySettings;

        settingsUI.style.display = DisplayStyle.None;
    }
}
