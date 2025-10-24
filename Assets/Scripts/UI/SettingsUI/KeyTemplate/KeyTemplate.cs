using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class KeyTemplate : VisualElement
{
    public SettingsScript SettingsScript;

    private bool isListening = false;

    private TextElement KeyName => this.Q<TextElement>("key-name");
    private Button KeyBtn => this.Q<Button>("key-btn");
    private Button ResetBtn => this.Q<Button>("key-reset-btn");

    public KeyCodeObj currentKeyCodeObj;
    public KeyCodeObj priorKeyCodeObj;
    public KeyCodeObj defaultKeyCodeObj;

    public void TemplateInit(KeyCodeObj keyObj, KeyCodeObj defaultObj)
    {
        currentKeyCodeObj = keyObj;
        priorKeyCodeObj = keyObj;
        defaultKeyCodeObj = defaultObj;

        KeyName.text = keyObj.name;

        KeyBtn.text = GetKeyString(keyObj.key);

        KeyBtn.clicked += KeyBtnClicked;
        ResetBtn.clicked += ResetBtnClicked;
    }

    string GetKeyString(KeyCode key)
    {
        return key switch
        {
            KeyCode.Mouse0 => "Left Click",
            KeyCode.Mouse1 => "Right Click",
            KeyCode.Mouse2 => "Middle Click",
            _ => key.ToString(),
        };
    }

    void KeyBtnClicked()
    {
        if (!isListening)
        {
            KeyBtn.text = "Press any key...";
            isListening = true;

            SettingsScript.ToggleAllBtnClickable(this);
            KeyBtn.RegisterCallback<KeyDownEvent>(OnKeyDown);
            KeyBtn.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }

    public void ResetBtnClicked()
    {
        currentKeyCodeObj = priorKeyCodeObj;


        KeyBtn.text = GetKeyString(priorKeyCodeObj.key);
    }

    public void ResetDefaultClicked()
    {
        KeyCodeObj[] keyArray = GameManager.Singleton.defaultPlayerSettings.keyArray;

        for (int i = 0; i < keyArray.Length; i++)
        {
            if (keyArray[i].name == currentKeyCodeObj.name)
            {
                currentKeyCodeObj = keyArray[i];

                KeyBtn.text = GetKeyString(keyArray[i].key);

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
            KeyBtn.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }

    void OnMouseDown(MouseDownEvent evt)
    {
        if (isListening)
        {
            switch (evt.button) 
            {
                case 0:
                    KeyBtn.text = "Left Click";
                    break;
                case 1:
                    KeyBtn.text = "Right Click";
                    break;
                case 2:
                    KeyBtn.text = "Middle Click";
                    break;
                default:
                    break;
            }

            currentKeyCodeObj.key = (KeyCode)(-evt.button - 1); // Convert mouse button to KeyCode
            isListening = false;
            SettingsScript.ToggleAllBtnClickable(this);
            KeyBtn.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            KeyBtn.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }

    public void OnBackClicked()
    {
        KeyBtn.clicked -= KeyBtnClicked;
        ResetBtn.clicked -= ResetBtnClicked;

        KeyBtn.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        KeyBtn.UnregisterCallback<MouseDownEvent>(OnMouseDown);
    }

    public void ToggleBtnClickable(KeyTemplate template)
    {
        if (template != this) 
        {
            KeyBtn.SetEnabled(!KeyBtn.enabledInHierarchy);
        }
        else
        {
            Debug.Log(KeyBtn.clickable.activators.Count);
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

    public void UpdatePriorKey()
    {
        priorKeyCodeObj = currentKeyCodeObj;
    }

    public KeyTemplate() { }
}
