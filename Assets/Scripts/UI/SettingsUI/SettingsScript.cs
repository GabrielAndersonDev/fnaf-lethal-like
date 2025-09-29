using UnityEngine;
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

    Button keySettingsBtn;
    Toggle playerlistToggle;
    Toggle sprintToggle;
    Toggle crouchToggle;
    Button applyBtn;
    Button backBtn;

    VisualElement keyBindingsContainer;
    Button resetToDefaultKeysBtn;
    Button backFromKeySettingsBtn;

    private void InitSettingsUI()
    {
        settingsUI = settingsObj.GetComponent<UIDocument>().rootVisualElement;

        settingsBox = settingsUI.Q<Box>("settings-box");
        keySettingsBox = settingsUI.Q<Box>("key-settings-container");

        keySettingsBtn = settingsBox.Q<Button>("key-settings-btn");
        playerlistToggle = settingsBox.Q<Toggle>("playerlist-toggle");
        sprintToggle = settingsBox.Q<Toggle>("sprint-toggle");
        crouchToggle = settingsBox.Q<Toggle>("crouch-toggle");
        applyBtn = settingsBox.Q<Button>("apply-btn");
        backBtn = settingsBox.Q<Button>("back-btn");

        keyBindingsContainer = keySettingsBox.Q<VisualElement>("key-bindings-container");
        resetToDefaultKeysBtn = keySettingsBox.Q<Button>("reset-btn");
        backFromKeySettingsBtn = keySettingsBox.Q<Button>("back-from-key-settings-btn");

        keySettingsBtn.clicked += OpenKeySettings;
        applyBtn.clicked += ApplySettings;
        backBtn.clicked += CloseSettings;
        resetToDefaultKeysBtn.clicked += ResetToDefaultKeys;
        backFromKeySettingsBtn.clicked += CloseKeySettings;
        playerlistToggle.value = SettingsManager.Singleton.showPlayerList;
        sprintToggle.value = SettingsManager.Singleton.sprintToRun;
        crouchToggle.value = SettingsManager.Singleton.crouchToWalk;
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
}
