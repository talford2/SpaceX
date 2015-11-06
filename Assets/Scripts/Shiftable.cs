using UnityEngine;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;

	public delegate void OnShiftEvent(CellIndex delta);
	public event OnShiftEvent OnShift;

	private Vector3 lastPosition;

	private void Awake()
	{
		Universe.ShiftableItems.Add(this);
	}

	private CellIndex CellIndexFromPosition(Vector3 position)
	{
		var cellZero = (position - Vector3.one * Universe.Current.CellSize / 2f) / Universe.Current.CellSize;
		return new CellIndex(Mathf.CeilToInt(cellZero.x), Mathf.CeilToInt(cellZero.y), Mathf.CeilToInt(cellZero.z));
	}

	public void Update()
	{
		var curCell = CellIndexFromPosition(transform.position);
		var lastCell = CellIndexFromPosition(lastPosition);
		var deltaCell = curCell - lastCell;

		//Debug.Log("DELTACELL: " + deltaCell);

		if (!deltaCell.IsZero())
		{
			UniverseCellIndex += deltaCell;
			if (OnShift != null)
				OnShift(deltaCell);
		}

		lastPosition = transform.position;
	}

	private void OnDestroy()
	{
		Universe.ShiftableItems.Remove(this);
	}

	public void Shift(Vector3 shiftAmount)
	{
		transform.position -= shiftAmount;
		lastPosition = transform.position;
	}
}