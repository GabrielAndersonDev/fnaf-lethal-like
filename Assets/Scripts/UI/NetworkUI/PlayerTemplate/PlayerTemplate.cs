using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PlayerTemplate : VisualElement
{
    private Image PlayerSprite => this.Q<Image>("player-img");
    private TextElement PlayerName => this.Q<TextElement>("player-name");
    private TextElement PlayerId => this.Q<TextElement>("player-id");

    public void TemplateInit(Texture2D texture, string name, ulong id)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        PlayerSprite.sprite = sprite;
        PlayerName.text = name;
        PlayerId.text = id.ToString();
    }

    public PlayerTemplate() { }
}
