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
        var playerPin = AddPin(PlayerPinPrefab, PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition());
        _mapCameraController.SetLookAt(playerPin.position);

        foreach (var squadronMember in PlayerController.Current.Squadron)
        {
            var squadronVehicle = squadronMember.VehicleInstance;
            if (squadronVehicle != null && squadronVehicle != PlayerController.Current.VehicleInstance)
            {
                AddPin(SquadronPinPrefab, squadronVehicle.Shiftable.GetWorldPosition());
            }
        }

        var universeEvents = Universe.Current.UniverseEvents;
        foreach (var universeEvent in universeEvents)
        {
            AddPin(PinPrefab, universeEvent.Shiftable.GetWorldPosition());
        }
    }

    private Transform AddPin(GameObject prefab, Vector3 worldPosition)
    {
        var mapScale = 0.01f;
        var pin = Utility.InstantiateInParent(prefab, transform);
        pin.layer = LayerMask.NameToLayer("Map");
        pin.transform.position = mapScale * worldPosition;
        return pin.transform;
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
