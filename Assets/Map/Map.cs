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
    private MapCamera _mapCameraController;
    private static Map _current;

    private List<Transform> _pins;

    public static Map Current
    {
        get { return _current; }
    }

    private void Awake()
    {
        _mapCamera = GetComponentInChildren<Camera>();
        _mapCameraController = _mapCamera.GetComponent<MapCamera>();
        _current = this;
        Hide();
    }

    private void Populate()
    {
        _pins = new List<Transform>();

        var playerPin = CreatePin(PlayerPinPrefab, PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition());
        _mapCameraController.SetLookAt(playerPin.position);
        _pins.Add(playerPin);

        foreach (var squadronMember in PlayerController.Current.Squadron)
        {
            var squadronVehicle = squadronMember.VehicleInstance;
            if (squadronVehicle != null && squadronVehicle != PlayerController.Current.VehicleInstance)
            {
                _pins.Add(CreatePin(SquadronPinPrefab, squadronVehicle.Shiftable.GetWorldPosition()));
            }
        }

        var universeEvents = Universe.Current.UniverseEvents;
        foreach (var universeEvent in universeEvents)
        {
            _pins.Add(CreatePin(PinPrefab, universeEvent.Shiftable.GetWorldPosition()));
        }
    }

    private Transform CreatePin(GameObject prefab, Vector3 worldPosition)
    {
        var mapScale = 0.01f;
        var pin = Utility.InstantiateInParent(prefab, transform);
        pin.layer = LayerMask.NameToLayer("Map");
        pin.transform.position = mapScale * worldPosition;
        return pin.transform;
    }

    public void Show()
    {
        PlayerController.Current.SetControlEnabled(false);
        Populate();
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
