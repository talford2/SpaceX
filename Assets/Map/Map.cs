using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public float MapScale = 0.01f;
    public Text MapSystemText;
    public GameObject PlayerPinPrefab;
    public GameObject SquadronPinPrefab;
    public GameObject PinPrefab;
    public GameObject DestinationPrefab;
    public Image DestinationImage;

    private Camera _mapCamera;
    private Canvas _mapCanvas;
    private static Map _current;

    private GameObject _playerPin;
    private List<GameObject> _squadronPins;
    private List<MapPin> _pins;

    private GameObject _destination;

    private List<Transform> _pinTransforms;

    public static Map Current
    {
        get { return _current; }
    }

    private void Awake()
    {
        _mapCamera = GetComponentInChildren<Camera>();
        _mapCanvas = GetComponentInChildren<Canvas>();
        _current = this;
        _playerPin = CreatePin(PlayerController.Current.PlayerPinPrefab);
        _destination = Instantiate(DestinationPrefab);
        _destination.SetActive(false);
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
        if (IsShown())
        {
            if (PlayerController.Current.VehicleInstance != null)
                _playerPin.transform.position = MapScale*PlayerController.Current.VehicleInstance.Shiftable.GetAbsoluteUniversePosition();
            foreach (var pin in _pins)
            {
                pin.PinInstance.transform.position = MapScale*pin.Shiftable.GetAbsoluteUniversePosition();
            }

            var mouseRay = _mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, LayerMask.GetMask("Map")))
            {
                Debug.Log("MOUSEOVER: " + mouseHit.collider.name);
                if (Input.GetMouseButtonUp(0))
                {
                    var clickedPin = _pins.First(p => p.PinInstance.transform == mouseHit.collider.transform.parent.transform);
                    Debug.Log("CLICKED: " + clickedPin.name);
                    _destination.SetActive(true);
                    _destination.GetComponent<Shiftable>().SetShiftPosition(clickedPin.Shiftable.UniversePosition);
                    _destination.transform.position = clickedPin.Shiftable.GetWorldPosition();
                }
            }
        }
    }

    private void LateUpdate()
    {
        DestinationImage.rectTransform.localPosition = _mapCamera.WorldToScreenPoint(MapScale * _destination.GetComponent<Shiftable>().GetAbsoluteUniversePosition()) - new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
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
        if (UniverseTrackers.Current != null)
            UniverseTrackers.Current.gameObject.SetActive(false);
        _mapCanvas.enabled = true;
    }

    public void Hide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _mapCamera.enabled = false;
        PlayerController.Current.SetControlEnabled(true);
        if (UniverseTrackers.Current != null)
            UniverseTrackers.Current.gameObject.SetActive(true);
        _mapCanvas.enabled = false;
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
