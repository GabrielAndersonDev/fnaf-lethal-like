using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PlayerTemplate : VisualElement
{
    private Image PlayerSprite => this.Q<Image>("player-img");
    private TextElement PlayerName => this.Q<TextElement>("player-name");
    private TextElement PlayerId => this.Q<TextElement>("player-id");

    public void TemplateInit(Texture2D sprite, string name, ulong id)
    {
        PlayerSprite.style.backgroundImage = sprite;
        PlayerName.text = name;
        PlayerId.text = id.ToString();
    }

    public PlayerTemplate() { }
}
