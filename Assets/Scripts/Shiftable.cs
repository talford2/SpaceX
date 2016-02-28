﻿using UnityEngine;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;
	public Vector3 CellLocalPosition;

	public UniversePosition UniversePosition
	{
		get { return new UniversePosition(UniverseCellIndex, CellLocalPosition); }
	}

	public delegate void OnCellIndexChangeEvent(Shiftable sender, CellIndex delta);

	public event OnCellIndexChangeEvent OnCellIndexChange;

	public delegate void OnShiftEvent(Shiftable sender, Vector3 delta);

	public event OnShiftEvent OnShift;

	// Performance variables
	private Vector3 destination = Vector3.zero;
	private CellIndex cellDelta;
	public void Translate(Vector3 translation)
	{
		destination = (CellLocalPosition + translation);
		cellDelta = CellIndexDeltaFromPosition(destination);

		if (!cellDelta.IsZero())
		{
			UniverseCellIndex += cellDelta;
			if (OnCellIndexChange != null)
			{
				OnCellIndexChange(this, cellDelta);
			}
			CellLocalPosition -= cellDelta.ToVector3() * Universe.Current.CellSize;
		}

		CellLocalPosition += translation;
		transform.position += translation;
	}

	private void Awake()
	{
		Universe.Current.ShiftableItems.Add(this);
	}

	private void Start()
	{
		transform.position = GetWorldPosition();
	}

	//private static Vector3 _cellDeltaCalc = Vector3.one * Universe.Current.HalfCellSize;
	private Vector3 _cellZero;

	private CellIndex _curCellIndex = new CellIndex();

	private CellIndex CellIndexDeltaFromPosition(Vector3 position)
	{
		_cellZero = (position - Vector3.one * Universe.Current.HalfCellSize) / Universe.Current.CellSize;
		_curCellIndex.Set(Mathf.CeilToInt(_cellZero.x), Mathf.CeilToInt(_cellZero.y), Mathf.CeilToInt(_cellZero.z));
		return _curCellIndex;
	}

	private void OnDestroy()
	{
		Universe.Current.ShiftableItems.Remove(this);
	}

	public void Shift(Vector3 shiftAmount)
	{
		transform.position -= shiftAmount;

		if (OnShift != null)
			OnShift(this, shiftAmount);
	}

	public void SetShiftPosition(UniversePosition position)
	{
		UniverseCellIndex = position.CellIndex;
		CellLocalPosition = position.CellLocalPosition;
	}

	public Vector3 GetWorldPosition()
	{
		return Universe.Current.GetWorldPosition(UniversePosition);
	}

	public Vector3 GetAbsoluteUniversePosition()
	{
		return Universe.Current.GetAbsoluteUniversePosition(UniversePosition);
	}
}