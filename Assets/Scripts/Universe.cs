using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
	public static CellIndex[,,] UniverseCells;

	public float CellSize = 200f;

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
		UniverseCells = new CellIndex[100, 100, 100];
		_current = this;
	}

	public void Start()
	{
		Debug.Log("Universe start");

		//// Move the player to the start position
		PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex = PlayerSpawnPosition.UniverseCellIndex;
		PlayerController.Current.VehicleInstance.transform.position = PlayerSpawnPosition.transform.position;

		PlayerController.Current.VehicleInstance.Shiftable.OnCellIndexChange += Shift;
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
			//var diff = PlayerSpawnPosition.UniverseCellIndex - PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
			//PlayerController.Current.VehicleInstance.transform.position = PlayerSpawnPosition.transform.position;
			//PlayerController.Current.VehicleInstance.transform.rotation = PlayerSpawnPosition.transform.rotation;
			//Shift(diff);
			Warp(PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex, PlayerSpawnPosition.transform);
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * CellSize);


		//for (var x = 0; x < 100; x++)
		//{
		//	for (var y = 0; y < 100; y++)
		//	{
		//		for (var z = 0; z < 100; z++)
		//		{

		//		}
		//	}
		//}
	}

	private void OnGUI()
	{
		var cellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
		GUI.Label(new Rect(50f, 50f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", cellIndex.X, cellIndex.Y, cellIndex.Z));
	}
}