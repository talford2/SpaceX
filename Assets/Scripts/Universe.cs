using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour
{
	public static CellIndex[,,] UniverseCells;

	public float CellSize = 200f;

	public Shiftable PlayerSpawnPosition;

	public void Awake()
	{
		UniverseCells = new CellIndex[100, 100, 100];
	}

	public void Start()
	{
		// Move the player to the start position
		PlayerController.Current.PlayerVehicle.Shiftable.UniverseCellIndex = PlayerSpawnPosition.UniverseCellIndex;
		PlayerController.Current.PlayerVehicle.Shiftable.CellPosition = PlayerSpawnPosition.CellPosition;
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