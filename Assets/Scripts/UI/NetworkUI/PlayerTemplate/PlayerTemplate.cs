using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public class PlayerTemplate : VisualElement
{
    private Image playerSprite => this.Q<Image>("player-img");
    private TextElement playerName => this.Q<TextElement>("player-name");
    private TextElement playerId => this.Q<TextElement>("player-id");

    public void TemplateInit(Texture2D sprite, string name, ulong id)
    {
        playerSprite.style.backgroundImage = sprite;
        playerName.text = name;
        playerId.text = id.ToString();
    }

    public PlayerTemplate() { }
}
