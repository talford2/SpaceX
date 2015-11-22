using UnityEngine;

public class HeadsUpDisplay : MonoBehaviour
{
    public GameObject Crosshair;

    private static HeadsUpDisplay _current;

    public static HeadsUpDisplay Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
    }

    public void ShowCrosshair()
    {
        Crosshair.SetActive(true);
    }

    public void HideCrosshair()
    {
        Crosshair.SetActive(false);
    }
}
