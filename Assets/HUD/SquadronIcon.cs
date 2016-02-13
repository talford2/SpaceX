using UnityEngine;
using UnityEngine.UI;

public class SquadronIcon : MonoBehaviour
{
    public Image Icon;
    public Image HealthBar;

    public Texture2D Default;
    public Texture2D Selected;
    public Texture2D Dead;

    private bool _isSelected;

    public void SetHealthFraction(float fraction)
    {
        HealthBar.fillAmount = fraction;
        if (fraction <= 0f)
        {
            Icon.overrideSprite = Sprite.Create(Dead, new Rect(0, 0, 70f, 70f), new Vector2(0.5f, 0.5f));
        }
        else
        {
            AssignLivingIcons();
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        AssignLivingIcons();
    }

    private void AssignLivingIcons()
    {
        var texture = Default;
        if (_isSelected)
            texture = Selected;
        Icon.overrideSprite = Sprite.Create(texture, new Rect(0, 0, 70f, 70f), new Vector2(0.5f, 0.5f));
    }
}
