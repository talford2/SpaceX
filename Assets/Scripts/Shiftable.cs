using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shiftable : MonoBehaviour
{
	[Tooltip("Cell the object is contained in")]
	public CellIndex UniverseCellIndex;

	public delegate void OnCellIndexChangeEvent(CellIndex delta);
	public event OnCellIndexChangeEvent OnCellIndexChange;

	public delegate void OnShiftEvent(Vector3 delta);
	public event OnShiftEvent OnShift;

	public ParticleSystem[] _particleSystems;

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

	private void Awake()
	{
		Universe.ShiftableItems.Add(this);

		//_particleSystems = GetComponentsInChildren<ParticleSystem>();
	}

	private CellIndex CellIndexFromPosition(Vector3 position)
	{
		var cellZero = (position - Vector3.one * Universe.Current.HalfCellSize) / Universe.Current.CellSize;
		return new CellIndex(Mathf.CeilToInt(cellZero.x), Mathf.CeilToInt(cellZero.y), Mathf.CeilToInt(cellZero.z));
	}

	private void OnDestroy()
	{
		Universe.ShiftableItems.Remove(this);
	}

	public void Shift(Vector3 shiftAmount)
	{
		transform.position -= shiftAmount;

		if (OnShift != null)
			OnShift(shiftAmount);

		foreach (var ps in _particleSystems)
		{
			Utility.MoveParticles(ps, Vector3.zero - shiftAmount);
			//Utility.MoveParticles(ps, shiftAmount);
		}
	}

	public Vector3 GetWorldPosition()
	{
		return Universe.Current.GetWorldPosition(UniverseCellIndex, CellLocalPosition);
	}
}