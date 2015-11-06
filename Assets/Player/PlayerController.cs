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
            _playVehicleInstance.VerticalTurn = Input.GetAxis("Vertical")*-1;
        }
        else
        {
            _playVehicleInstance.VerticalTurn = Input.GetAxis("Vertical");
        }
        _playVehicleInstance.HorizontalTurn = Input.GetAxis("Horizontal");

        //_playVehicleInstance.CurrentWeapon.IsTriggered = true;
        _playVehicleInstance.CurrentWeapon.IsTriggered = Input.GetAxis("FireTrigger") > 0;
        //Debug.Log(_playVehicleInstance.CurrentWeapon.IsTriggered);
        //if (Input.GetAxis("FireTrigger") != 0)
        //{
        //	Debug.LogFormat("FireTrigger 1 : {0}", Input.GetAxis("FireTrigger"));
        //	_playVehicleInstance.CurrentWeapon.IsTriggered = true;
        //}


        //var vehicle

        // Check for shifting
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
