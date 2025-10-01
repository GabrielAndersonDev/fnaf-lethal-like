using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class KeyTemplate : VisualElement
{
    public SettingsScript SettingsScript { get; private set; }

    private bool isListening = false;

    private TextElement KeyName => this.Q<TextElement>("key-name");
    private Button KeyBtn => this.Q<Button>("key-btn");
    private Button ResetBtn => this.Q<Button>("key-reset-btn");

    public KeyCodeObj currentKeyCodeObj;
    public KeyCodeObj priorKeyCodeObj;

    public void TemplateInit(KeyCodeObj keyObj)
    {
        currentKeyCodeObj = keyObj;
        priorKeyCodeObj = keyObj;

        KeyName.text = keyObj.name;
        KeyBtn.text = keyObj.key.ToString();

        KeyBtn.clicked += KeyBtnClicked;
        ResetBtn.clicked += ResetBtnClicked;
    }

    void KeyBtnClicked()
    {
        Debug.Log("Key code button clicked!");
        if (!isListening)
        {
            KeyBtn.text = "Press any key...";
            isListening = true;

            SettingsScript.ToggleAllBtnClickable(this);
            KeyBtn.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    public void ResetBtnClicked()
    {
        currentKeyCodeObj = priorKeyCodeObj;
        KeyBtn.text = priorKeyCodeObj.key.ToString();
    }

    public void ResetDefaultClicked()
    {
        KeyCodeObj[] keyArray = GameManager.Singleton.defaultPlayerSettings.keyArray;

        for (int i = 0; i < keyArray.Length; i++)
        {
            if (keyArray[i].name == currentKeyCodeObj.name)
            {
                currentKeyCodeObj = keyArray[i];
                KeyBtn.text = keyArray[i].key.ToString();
                return;
            }
        }
    }

    public bool IsDifferentFromPrior()
    {
        if (currentKeyCodeObj.key != priorKeyCodeObj.key)
        {
            return true;
        }
        return false;
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        if (isListening)
        {
            KeyBtn.text = evt.keyCode.ToString();
            currentKeyCodeObj.key = evt.keyCode;
            isListening = false;
            SettingsScript.ToggleAllBtnClickable(this);
            KeyBtn.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    public void OnBackClicked()
    {
        KeyBtn.clicked -= KeyBtnClicked;
        ResetBtn.clicked -= ResetBtnClicked;

        KeyBtn.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void ToggleBtnClickable(KeyTemplate template)
    {
        if (template != this) 
        {
            KeyBtn.SetEnabled(!KeyBtn.enabledInHierarchy);
        }
        else
        {
            if (KeyBtn.clickable.activators.Count != 0)
            {
                KeyBtn.clickable.activators.Clear();
            }
            else
            {
                KeyBtn.clickable.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            }
        }

        ResetBtn.SetEnabled(!ResetBtn.enabledInHierarchy);
    } 

    public KeyTemplate() { }
}
