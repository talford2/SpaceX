using UnityEngine;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;

	public delegate void OnCellIndexChangeEvent(CellIndex delta);
	public event OnCellIndexChangeEvent OnCellIndexChange;

	public delegate void OnShiftEvent(Vector3 delta);
	public event OnShiftEvent OnShift;

	private Vector3 _cellLocalPosition;

    public Vector3 CellLocalPosition { get { return _cellLocalPosition; } }

	public void Translate(Vector3 translation)
	{
		var destination = (_cellLocalPosition + translation);
		var cellDelta = CellIndexFromPosition(destination);
		
		if (!cellDelta.IsZero())
		{
			UniverseCellIndex += cellDelta;
			if (OnCellIndexChange != null)
			{
				OnCellIndexChange(cellDelta);
			}
			_cellLocalPosition -= cellDelta.ToVector3() * Universe.Current.CellSize;
		}

		_cellLocalPosition += translation;
		transform.position += translation;
	}

	private Vector3 lastPosition;

	private void Awake()
	{
		Universe.ShiftableItems.Add(this);
	}

	private CellIndex CellIndexFromPosition(Vector3 position)
	{
		var cellZero = (position - Vector3.one * Universe.Current.HalfCellSize) / Universe.Current.CellSize;
		return new CellIndex(Mathf.CeilToInt(cellZero.x), Mathf.CeilToInt(cellZero.y), Mathf.CeilToInt(cellZero.z));
	}

	//public void UpdateCell()
	//{
	//	var curCell = CellIndexFromPosition(transform.position);
	//	var lastCell = CellIndexFromPosition(lastPosition);
	//	var deltaCell = curCell - lastCell;

	//	if (!deltaCell.IsZero())
	//	{
	//		UniverseCellIndex += deltaCell;
	//		if (OnCellIndexChange != null)
	//			OnCellIndexChange(deltaCell);
	//	}

	//	lastPosition = transform.position;
	//}

	private void OnDestroy()
	{
		Universe.ShiftableItems.Remove(this);
	}

	public void Shift(Vector3 shiftAmount)
	{
		var diff = lastPosition - transform.position;

		transform.position -= shiftAmount;
		lastPosition = transform.position + diff;

		if (OnShift != null)
			OnShift(shiftAmount);
	}
}