using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		//Debug.Log(PlayerController.Current.PlayerVehicle.Shiftable);

		//// Move the player to the start position
		PlayerController.Current.PlayerVehicle.Shiftable.UniverseCellIndex = PlayerSpawnPosition.UniverseCellIndex;
		PlayerController.Current.PlayerVehicle.transform.position = PlayerSpawnPosition.transform.position;
	}

	public void Update()
	{
		// Update cell
		foreach (var shiftable in ShiftableItems)
		{
			var prevCell = shiftable.UniverseCellIndex;

			var delta = (shiftable.transform.position - (Vector3.one * CellSize * 0.5f)) / (CellSize);

			//var newCell = 
			shiftable.UniverseCellIndex += new CellIndex(Mathf.CeilToInt(delta.x), Mathf.CeilToInt(delta.y), Mathf.CeilToInt(delta.z));

			if (prevCell.X != shiftable.UniverseCellIndex.X || prevCell.Y != shiftable.UniverseCellIndex.Y || prevCell.Z != shiftable.UniverseCellIndex.Z)
			{
				Debug.LogFormat("Cell change from {0} to {1}", prevCell, shiftable.UniverseCellIndex);
				Shift();
			}

			//var vDelta = new Vector3(Mathf.CeilToInt())

		}
	}

	public void Shift()
	{
		foreach (var shiftable in ShiftableItems)
		{
			Debug.LogFormat("Shift {0}", shiftable.name);
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
}