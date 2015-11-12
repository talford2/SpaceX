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

    private float screenAspect;

    private void Awake()
    {
        _playVehicleInstance = Instantiate<Vehicle>(VehiclePrefab);
        _current = this;
        Cursor.visible = !HideMouse;
        if (HideMouse)
            Cursor.lockState = CursorLockMode.Locked;

        PlayerVehicles.Insert(0, _playVehicleInstance);

        screenAspect = (float)Screen.width/(float)Screen.height;

        Debug.Log("WIDTH: " + Screen.width);
        Debug.Log("HEIGHT: " + Screen.height);

        Debug.Log("ASPECT: " + screenAspect);
        //Debug.Break();
    }

    private void Start()
	{
        //Universe.Current.ViewPort.Tar
		//FollowCamera.Current.Target = _playVehicleInstance.transform;
	}

    private void Update()
    {
        var mouseVertical = 1000f*(Input.GetAxis("MouseVertical")/Screen.width)*screenAspect;
        var mouseHorizontal = 1000f*Input.GetAxis("MouseHorizontal")/Screen.height;

        if (InvertY)
        {
            _playVehicleInstance.PitchThotttle = (Input.GetAxis("Vertical") + mouseVertical) * -1;
        }
        else
        {
            _playVehicleInstance.PitchThotttle = Input.GetAxis("Vertical") + mouseVertical;
        }
        _playVehicleInstance.YawThrottle = Input.GetAxis("Horizontal") + mouseHorizontal;
        _playVehicleInstance.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
        _playVehicleInstance.CurrentWeapon.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;

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
}
