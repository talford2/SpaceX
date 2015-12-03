using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Text MapSystemText;

    private Camera _mapCamera;
    private static Map _current;

    public static Map Current
    {
        get { return _current; }
    }

    private void Awake()
    {
        _mapCamera = GetComponentInChildren<Camera>();
        _current = this;
        Hide();
    }

    public void Show()
    {
        _mapCamera.enabled = true;
    }

    public void Hide()
    {
        _mapCamera.enabled = false;
    }

    public void Toggle()
    {
        _mapCamera.enabled = !_mapCamera.enabled;
    }
}
