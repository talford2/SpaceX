using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
	public float MapScale = 0.01f;
	public Text MapSystemText;
	public GameObject DestinationPrefab;
	public Image DestinationImage;
	public Material GridLineMaterial;

	private Camera _mapCamera;
	private Canvas _mapCanvas;
	private static Map _current;

	//private GameObject _playerPin;
	private List<MapPin> _pins;

	private bool isDestinationSet;
	private GameObject _destination;
	
	public static Map Current
	{
		get { return _current; }
	}

	private void Awake()
	{
		_mapCamera = GetComponentInChildren<Camera>();
		_mapCanvas = GetComponentInChildren<Canvas>();
		_current = this;
		//_playerPin = CreatePin(PlayerController.Current.PlayerPinPrefab);
		_destination = Instantiate(DestinationPrefab);
		_destination.SetActive(false);
		isDestinationSet = false;

		GetComponentInChildren<MapCamera>().OnMove += UpdateDestination;

		Hide();
	}

	private void Start()
	{
		//CreateGrid();
	}

	public void AddPin(MapPin pin)
	{
		if (_pins == null)
			_pins = new List<MapPin>();

		pin.ActiveInstance = CreatePin(pin.ActivePin);
		if (pin.ActiveInstance.GetComponentInChildren<Billboard>())
			pin.ActiveInstance.GetComponentInChildren<Billboard>().UseCamera = _mapCamera;

		pin.InactiveInstance = CreatePin(pin.InactivePin);
		if (pin.InactiveInstance.GetComponentInChildren<Billboard>())
			pin.InactiveInstance.GetComponentInChildren<Billboard>().UseCamera = _mapCamera;

		_pins.Add(pin);

	}

	public void RemovePin(MapPin pin)
	{
		_pins.Remove(pin);
		if (pin.ActiveInstance != null)
			Destroy(pin.ActiveInstance);
	}

    private void UpdateMap()
    {
        if (IsShown())
        {
            foreach (var pin in _pins)
            {
                pin.RenderState();
                pin.CurrentInstance.transform.position = MapScale * pin.Shiftable.GetAbsoluteUniversePosition();
                pin.CurrentInstance.transform.rotation = pin.transform.rotation;
                
                var toCamera = pin.CurrentInstance.transform.position - MapCamera.Current.transform.position;
                if (toCamera.sqrMagnitude > 10000f)
                {
                    pin.CurrentInstance.transform.localScale = Vector3.one * toCamera.magnitude /100f;
                    //= Vector3.one * toCamera.magnitude * 0.01f;
                }
                else
                {
                    pin.CurrentInstance.transform.localScale = Vector3.one;
                }
                
            }

            var mouseRay = _mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, LayerMask.GetMask("Map")))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    var clickedPin = _pins.First(p => p.CurrentInstance.transform == mouseHit.collider.transform.parent.transform);
                    _destination.SetActive(true);
                    isDestinationSet = true;
                    _destination.GetComponent<Shiftable>().SetShiftPosition(clickedPin.Shiftable.UniversePosition);
                    _destination.transform.position = clickedPin.Shiftable.GetWorldPosition();
                }
                else
                {

                }
            }
        }
    }

	private void UpdateDestination()
	{
		if (isDestinationSet)
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
        Time.timeScale = 0f;
		PlayerController.Current.SetControlEnabled(false);

        var centre = MapScale * PlayerController.Current.VehicleInstance.Shiftable.GetAbsoluteUniversePosition();
		MapCamera.Current.SetLookAt(centre);

		_mapCamera.enabled = true;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		_mapCanvas.enabled = true;

        MapSystemText.text = LevelManager.Current.GetLevel().SystemName;

		if (TrackerManager.Current != null)
			TrackerManager.Current.SetTrackersVisibility(false);

        MapCamera.Current.OnMove += UpdateMap;
	}

    public void Hide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _mapCamera.enabled = false;
        PlayerController.Current.SetControlEnabled(true);
        _mapCanvas.enabled = false;
        if (TrackerManager.Current != null)
            TrackerManager.Current.SetTrackersVisibility(true);

        Time.timeScale = 1f;
        if (MapCamera.Current != null)
            MapCamera.Current.OnMove -= UpdateMap;
    }

    public void Toggle()
	{
		if (_mapCamera.enabled)
		{
			Hide();
			gameObject.SetActive(false);
		}
		else
		{
			Show();
			gameObject.SetActive(true);
		}
	}

	public bool IsShown()
	{
		return _mapCamera.enabled;
	}

	private void CreateGrid()
	{
		var grid = new GameObject();
		grid.transform.SetParent(transform);
		grid.name = "Grid";
		grid.layer = LayerMask.NameToLayer("Map");

		var radius = 130f;
		var gridCount = 16;

		float gridStep = radius * 2f / (float)gridCount;

		float lineWidth = 0.25f;
		for (var x = 0; x <= gridCount; x++)
		{
			var xPos = radius - x * gridStep;

			var line1 = new GameObject();
			line1.transform.SetParent(grid.transform);
			line1.name = "LineX" + x;
			line1.layer = LayerMask.NameToLayer("Map");
			var lr = line1.AddComponent<LineRenderer>();
			lr.material = GridLineMaterial;
			lr.SetWidth(lineWidth, lineWidth);
			lr.SetVertexCount(3);

			lr.SetPositions(new Vector3[] {
				new Vector3(xPos, 0, -radius),
				new Vector3(xPos, 0, 0),
				new Vector3(xPos, 0, radius)
			});

			var line2 = new GameObject();
			line2.transform.SetParent(grid.transform);
			line2.name = "LineZ" + x;
			line2.layer = LayerMask.NameToLayer("Map");
			var lr2 = line2.AddComponent<LineRenderer>();
			lr2.material = GridLineMaterial;
			lr2.SetWidth(lineWidth, lineWidth);
			lr2.SetVertexCount(3);

			lr2.SetPositions(new Vector3[] {
				new Vector3(-radius, 0 ,xPos),
				new Vector3(0, 0 ,xPos),
				new Vector3(radius, 0, xPos)
			});
		}
	}
}
