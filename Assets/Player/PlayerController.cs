﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public Vehicle PlayerVehicle;
	
	private Vehicle _playVehicleInstance;
	
	public bool InvertY = false;

	void Awake()
	{
		_playVehicleInstance = Instantiate<Vehicle>(PlayerVehicle);
		_current = this;
	}

	void Start()
	{
		FollowCamera.Current.Target = _playVehicleInstance.transform;
	}

	void Update()
	{
		if (InvertY)
		{
			_playVehicleInstance.VerticalTurn = Input.GetAxis("Vertical") * -1;
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
		get
		{
			return _current;
		}
	}

    private void OnGUI()
    {
        GUI.Label(new Rect(50f, 50f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", _playVehicleInstance.Shiftable.UniverseCellIndex.X, _playVehicleInstance.Shiftable.UniverseCellIndex.Y, _playVehicleInstance.Shiftable.UniverseCellIndex.Z));
    }
}
