using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
	public static CellIndex[,,] UniverseCells;

	public float CellSize = 200f;

	public Shiftable PlayerSpawnPosition;

    private CellIndex playerCellIndex;

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
		PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex = PlayerSpawnPosition.UniverseCellIndex;
        PlayerController.Current.VehicleInstance.transform.position = PlayerSpawnPosition.transform.position;

	    playerCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
        PlayerController.Current.VehicleInstance.Shiftable.OnShift += Shift;
	}

	public void Shift(CellIndex delta)
	{
	    //playerCellIndex += delta;
	    //PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex -= delta;
		foreach (var shiftable in ShiftableItems)
		{
			shiftable.transform.position -= delta.ToVector3() * CellSize;
			Debug.LogFormat("Shift {0} by {1}", shiftable.name, delta);
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
        GUI.Label(new Rect(50f, 50f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", playerCellIndex.X, playerCellIndex.Y, playerCellIndex.Z));
    }
}