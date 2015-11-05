using UnityEngine;
using System.Collections;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Position of object within its cell")]
	public Vector3 CellPosition;

	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;
}