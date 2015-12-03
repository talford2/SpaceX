using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Text MapSystemText;
    public GameObject PinPrefab;

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

    private void Populate()
    {
        var universeEvents = Universe.Current.UniverseEvents;
        foreach (var universeEvent in universeEvents)
        {
            var pin = Utility.InstantiateInParent(PinPrefab, transform);
            pin.layer = LayerMask.NameToLayer("Map");
            pin.transform.position = 0.01f*universeEvent.Shiftable.GetWorldPosition();
        }
    }

    public void Show()
    {
        _mapCamera.enabled = true;
        Cursor.visible = true;
        PlayerController.Current.SetControlEnabled(false);
        Populate();
    }

    public void Hide()
    {
        _mapCamera.enabled = false;
        Cursor.visible = false;
        PlayerController.Current.SetControlEnabled(true);
    }

    public void Toggle()
    {
        if (_mapCamera.enabled)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public bool IsShown()
    {
        return _mapCamera.enabled;
    }
}
