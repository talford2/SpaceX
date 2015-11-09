using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
	public float CellSize = 200f;

	public float HalfCellSize { get; private set; }

	public Shiftable PlayerSpawnPosition;

	public static List<Shiftable> ShiftableItems;

	private static Universe _current;
	public static Universe Current
	{
		get
		{
			return _current;
		}
	}

	public void Awake()
	{
		ShiftableItems = new List<Shiftable>();
		_current = this;
		HalfCellSize = CellSize / 2f;
	}

	public void Start()
	{
		Debug.Log("Universe start");

		//// Move the player to the start position
		PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex = PlayerSpawnPosition.UniverseCellIndex;
		PlayerController.Current.VehicleInstance.transform.position = PlayerSpawnPosition.transform.position;

	    FollowCamera.Current.Shiftable.OnCellIndexChange += Shift;
	}

	public void Shift(CellIndex delta)
	{
		foreach (var shiftable in ShiftableItems)
		{
			shiftable.Shift(delta.ToVector3() * CellSize);
		}
	}

	public void Warp(CellIndex cell, Transform trans)
	{
		var diff = cell - PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
		PlayerController.Current.VehicleInstance.transform.position = trans.position;
		PlayerController.Current.VehicleInstance.transform.rotation = trans.rotation;
		FollowCamera.Current.transform.rotation = trans.rotation;
		Shift(diff);
	}

	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			Warp(PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex, PlayerSpawnPosition.transform);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * CellSize);
	}

	private void OnGUI()
	{
		var cellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
		GUI.Label(new Rect(50f, 50f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", cellIndex.X, cellIndex.Y, cellIndex.Z));
	}
}