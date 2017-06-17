using UnityEngine;

public class MissionCompleteScreen : MonoBehaviour
{
    public CanvasGroup ScreenGroup;

    private static MissionCompleteScreen _current;

    public static MissionCompleteScreen Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
        Hide();
    }

    public void Show()
    {
        ScreenGroup.alpha = 1f;
    }

    public void Hide()
    {
        ScreenGroup.alpha = 0;
    }
}
