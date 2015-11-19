using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Vehicle VehiclePrefab;

	private Vehicle _playVehicleInstance;

	public List<Vehicle> PlayerVehicles;
	private int _curVehicleIndex = 0;

	public bool InvertY = false;
	public bool HideMouse = false;

    public Shiftable RespawnPosition;

	[Header("Aiming")]
	public float AimSensitivity = 10f;
	public float MinAimDistance = 10f;
	public float MaxAimDistance = 1000f;

	private float screenAspect;

	private void Awake()
	{
        SpawnVehicle(VehiclePrefab, Universe.Current.PlayerSpawnPosition.CellLocalPosition, Universe.Current.PlayerSpawnPosition.transform.rotation);
		_current = this;
		Cursor.visible = !HideMouse;
		if (HideMouse)
			Cursor.lockState = CursorLockMode.Locked;

		screenAspect = (float)Screen.height / (float)Screen.width;

		//Debug.Break();
	}

	private void Start()
	{
		//Universe.Current.ViewPort.Tar
		//FollowCamera.Current.Target = _playVehicleInstance.transform;
	}

    private void SpawnVehicle(Vehicle vehiclePrefab, Vector3 position, Quaternion rotation)
    {
        _playVehicleInstance = ((GameObject)Instantiate(vehiclePrefab.gameObject, position, rotation)).GetComponent<Vehicle>();
        Destroy(_playVehicleInstance.GetComponent<Tracker>());
        _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");
        PlayerVehicles.Insert(0, _playVehicleInstance);
    }

	private Vector3 GetAimAt()
	{
		var mouseRay = Universe.Current.ViewPort.AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit aimHit;
		var weaponShootPointsCentre = _playVehicleInstance.CurrentWeapon.GetShootPointCentre();
		var aimAtPosition = weaponShootPointsCentre + _playVehicleInstance.transform.forward * MaxAimDistance;
		if (Physics.Raycast(mouseRay, out aimHit, MaxAimDistance, ~LayerMask.GetMask("Player")))
		{
			var toAim = aimHit.point - weaponShootPointsCentre;
			var dotProd = Vector3.Dot(toAim, _playVehicleInstance.transform.forward);
			if (dotProd > MinAimDistance)
			{
				aimAtPosition = aimHit.point;
			}
		}
		return aimAtPosition;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Debug.Break();
		}
	    if (_playVehicleInstance != null)
	    {
	        var mouseClamp = 0.003f;
	        var mouseHorizontal = AimSensitivity*Mathf.Clamp(Input.GetAxis("MouseHorizontal")/Screen.width, -mouseClamp, mouseClamp);
	        var mouseVertical = AimSensitivity*Mathf.Clamp(screenAspect*Input.GetAxis("MouseVertical")/Screen.height, -mouseClamp, mouseClamp);

	        if (InvertY)
	        {
	            _playVehicleInstance.PitchThotttle = (Input.GetAxis("Vertical") + mouseVertical)*-1;
	        }
	        else
	        {
	            _playVehicleInstance.PitchThotttle = Input.GetAxis("Vertical") + mouseVertical;
	        }
	        _playVehicleInstance.YawThrottle = Input.GetAxis("Horizontal") + mouseHorizontal;
	        _playVehicleInstance.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
	        _playVehicleInstance.CurrentWeapon.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;

	        _playVehicleInstance.SetAimAt(GetAimAt());

	        _playVehicleInstance.TriggerAccelerate = false;
	        if (Input.GetButton("Accelerate") || Input.GetButton("KeyboardAccelerate"))
	        {
	            _playVehicleInstance.TriggerAccelerate = true;
	        }

	        _playVehicleInstance.TriggerBrake = false;
	        if (Input.GetButton("Brake") || Input.GetButton("KeyboardBrake"))
	        {
	            _playVehicleInstance.TriggerBrake = true;
	        }

	        _playVehicleInstance.TriggerBoost = false;
	        if (Input.GetButton("KeyboardBoost"))
	        {
	            _playVehicleInstance.TriggerBoost = true;
	        }
	    }

	    if (Input.GetKeyUp(KeyCode.R))
	    {
	        _playVehicleInstance.GetComponent<Killable>().Die();

            Debug.Log("RESTART");

            Universe.Current.ViewPort.Shiftable.UniverseCellIndex = RespawnPosition.UniverseCellIndex;
            Universe.Current.ViewPort.Shiftable.CellLocalPosition = RespawnPosition.CellLocalPosition;

            //var shiftDelta = RespawnPosition.UniverseCellIndex - Universe.Current.ViewPort.Shiftable.UniverseCellIndex;
            var shiftDelta = Universe.Current.ViewPort.Shiftable.UniverseCellIndex - RespawnPosition.UniverseCellIndex;

            Universe.Current.Shift(shiftDelta);

            SpawnVehicle(VehiclePrefab, RespawnPosition.CellLocalPosition, RespawnPosition.transform.rotation);

            _playVehicleInstance.Shiftable.UniverseCellIndex = RespawnPosition.UniverseCellIndex;
            _playVehicleInstance.Shiftable.CellLocalPosition = RespawnPosition.CellLocalPosition;

            Universe.Current.ViewPort.GetComponent<VehicleCamera>().Target = _playVehicleInstance;

            Debug.Log("NEW PLAYER POS: " + _playVehicleInstance.transform.position);
	    }

	    if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}

		if (Input.GetKeyUp(KeyCode.E))
		{
			_curVehicleIndex++;
			if (_curVehicleIndex >= PlayerVehicles.Count)
			{
				_curVehicleIndex = 0;
			}
			_playVehicleInstance = PlayerVehicles[_curVehicleIndex];
			//FollowCamera.Current.Target = _playVehicleInstance.transform;
		}

		//var vehicle
		// Check for shifting
	}

	public bool InPlayerActiveCells(CellIndex checkCell)
	{
		var playerCellIndex = _playVehicleInstance.Shiftable.UniverseCellIndex;
		for (var x = -1; x < 2; x++)
		{
			for (var y = -1; y < 2; y++)
			{
				for (var z = -1; z < 2; z++)
				{
					if (checkCell.IsEqualTo(playerCellIndex + new CellIndex(x, y, z)))
						return true;
				}
			}
		}
		return false;
	}

	private static PlayerController _current;

	public static PlayerController Current
	{
		get { return _current; }
	}

	public Vehicle VehicleInstance
	{
		get { return _playVehicleInstance; }
	}

    private void OnGUI()
    {
        //GUI.Label(new Rect(Screen.width - 100f, Screen.height - 100f, 100f, 25), string.Format("{0:f2} m/s", VehicleInstance.GetVelocity().magnitude));
        if (_playVehicleInstance == null)
        {
            GUI.Label(new Rect(Screen.width/2f - 100f, Screen.height/2f + 50f, 200f, 25f), "Press 'R' to respawn.", new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}});
        }
    }
}
