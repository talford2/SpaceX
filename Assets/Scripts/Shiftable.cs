using UnityEngine;
using System.Collections;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;

	void Awake()
	{
		Universe.ShiftableItems.Add(this);
	}
}