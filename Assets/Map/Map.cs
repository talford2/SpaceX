using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Text MapSystemText;
    public GameObject PlayerPinPrefab;
    public GameObject SquadronPinPrefab;
    public GameObject PinPrefab;

    private Camera _mapCamera;
    private static Map _current;

    private List<MapPin> _pins;

    private List<Transform> _pinTransforms;

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

    public void AddPin(MapPin pin)
    {
        if (_pins == null)
            _pins = new List<MapPin>();
        pin.PinInstance = CreatePin(pin.ActivePin);
        _pins.Add(pin);
        
    }

    public void RemovePin(MapPin pin)
    {
        _pins.Remove(pin);
        if (pin.gameObject != null)
            Destroy(pin.gameObject);
    }

    private void Update()
    {
        var mapScale = 0.01f;
        if (IsShown())
        {
            foreach (var pin in _pins)
            {
                pin.PinInstance.transform.position = mapScale*pin.transform.position;
            }
        }
    }

    private GameObject CreatePin(GameObject prefab)
    {
        var pin = Utility.InstantiateInParent(prefab, transform);
        pin.layer = LayerMask.NameToLayer("Map");
        return pin;
    }

    public void Show()
    {
        PlayerController.Current.SetControlEnabled(false);
        _mapCamera.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (_pins != null)
        {
            for (var i = 0; i < _pins.Count; i++)
            {
                Destroy(_pins[i].gameObject);
            }
        }
        _mapCamera.enabled = false;
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
