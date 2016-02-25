using System.Collections.Generic;
using UnityEngine;

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

	public List<ParticleSystem> ShiftParticleSystems;

	public void Translate(Vector3 translation)
	{
		var destination = (CellLocalPosition + translation);
		var cellDelta = CellIndexDeltaFromPosition(destination);

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
		//_particleSystems = GetComponentsInChildren<ParticleSystem>();
	}

	private void Start()
	{
		transform.position = GetWorldPosition();
	}

	private CellIndex CellIndexDeltaFromPosition(Vector3 position)
	{
		var cellZero = (position - Vector3.one * Universe.Current.HalfCellSize) / Universe.Current.CellSize;
		return new CellIndex(Mathf.CeilToInt(cellZero.x), Mathf.CeilToInt(cellZero.y), Mathf.CeilToInt(cellZero.z));
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

		//var particleShift = Vector3.zero - shiftAmount;
		//foreach (var ps in ShiftParticleSystems)
		//{
		//	Utility.MoveParticles(ps, particleShift);
		//}
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