using UnityEngine;
using System.Collections;

public class Universe : MonoBehaviour
{
	public static CellIndex[,,] UniverseCells;

	public float CellSize = 200f;

	public void Awake()
	{
		UniverseCells = new CellIndex[100, 100, 100];
	}

	public void OnDrawGizmos()
	{
		//Gizmos.color = Color.red;
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