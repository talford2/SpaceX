using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Vehicle VehiclePrefab;

	private Vehicle _playVehicleInstance;

	public bool InvertY = false;

	private void Awake()
	{
		_playVehicleInstance = Instantiate<Vehicle>(VehiclePrefab);
		_current = this;
	}

	private void Start()
	{
		FollowCamera.Current.Target = _playVehicleInstance.transform;
	}

	private void Update()
	{
		if (InvertY)
		{
			_playVehicleInstance.PitchThotttle = Input.GetAxis("Vertical") * -1;
		}
		else
		{
			_playVehicleInstance.PitchThotttle = Input.GetAxis("Vertical");
		}
		_playVehicleInstance.YawThrottle = Input.GetAxis("Horizontal");

		//_playVehicleInstance.CurrentWeapon.IsTriggered = true;
		_playVehicleInstance.CurrentWeapon.IsTriggered = Input.GetAxis("FireTrigger") > 0;
		//Debug.Log(_playVehicleInstance.CurrentWeapon.IsTriggered);
		//if (Input.GetAxis("FireTrigger") != 0)
		//{
		//	Debug.LogFormat("FireTrigger 1 : {0}", Input.GetAxis("FireTrigger"));
		//	_playVehicleInstance.CurrentWeapon.IsTriggered = true;
		//}

		var roll = Input.GetAxis("Roll");
		Debug.Log("roll = " + roll);


		_playVehicleInstance.IsAccelerating = false;
		if (Input.GetButton("Boost"))
		{
			Debug.Log("Boost!");
			_playVehicleInstance.IsAccelerating = true;
		}

		_playVehicleInstance.IsBraking = false;
		if (Input.GetButton("Brake"))
		{
			Debug.Log("Brake!");
			_playVehicleInstance.IsBraking = true;
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
