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

    private GameObject _playerPin;
    private List<GameObject> _squadronPins;
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
        _playerPin = CreatePin(PlayerController.Current.PlayerPinPrefab);
        Hide();
    }

    public void AddPin(MapPin pin)
    {
        if (_pins == null)
            _pins = new List<MapPin>();
        pin.PinInstance = CreatePin(pin.ActivePin);
        if (pin.PinInstance.GetComponentInChildren<Billboard>())
            pin.PinInstance.GetComponentInChildren<Billboard>().UseCamera = _mapCamera;
        _pins.Add(pin);
        
    }

    public void RemovePin(MapPin pin)
    {
        _pins.Remove(pin);
        if (pin.PinInstance != null)
            Destroy(pin.PinInstance);
    }

    private void Update()
    {
        var mapScale = 0.01f;
        if (IsShown())
        {
            if (PlayerController.Current.VehicleInstance != null)
                _playerPin.transform.position = mapScale*PlayerController.Current.VehicleInstance.Shiftable.GetAbsoluteUniversePosition();
            foreach (var pin in _pins)
            {
                pin.PinInstance.transform.position = mapScale*pin.Shiftable.GetAbsoluteUniversePosition();
            }

            var mouseRay = _mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, LayerMask.GetMask("Map")))
            {
                Debug.Log("MOUSEOVER: " + mouseHit.collider.name);
            }
        }
    }

    private GameObject CreatePin(GameObject prefab)
    {
        var pin = Utility.InstantiateInParent(prefab, transform);
        Utility.SetLayerRecursively(pin, LayerMask.NameToLayer("Map"));
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
