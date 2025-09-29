using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class KeyTemplate : VisualElement
{
    private bool isListening = false;
    private TextElement KeyName => this.Q<TextElement>("key-name");
    private Button KeyCode => this.Q<Button>("key-code-btn");
    public KeyCodeObj KeyCodeObj;

    public void TemplateInit(KeyCodeObj keyObj)
    {
        KeyName.text = keyObj.name;
        KeyCode.text = keyObj.key.ToString();

        KeyCode.clicked += KeyCodeClicked;
    }

    void KeyCodeClicked()
    {
        Debug.Log("Key code button clicked!");
        if (!isListening)
        {
            KeyCode.text = "Press any key...";
            isListening = true;
            KeyCode.clickable = null;
            KeyCode.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        if (isListening)
        {
            KeyCode.text = evt.keyCode.ToString();
            KeyCodeObj.key = evt.keyCode;
            isListening = false;
            KeyCode.clickable = new Clickable(KeyCodeClicked);
            KeyCode.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    public void OnBackClicked()
    {
        KeyCode.clicked -= KeyCodeClicked;
        KeyCode.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public KeyTemplate() { }
}
