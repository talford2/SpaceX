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

	public delegate void OnShiftEvent(Vector3 delta);

	public event OnShiftEvent OnShift;

	public ParticleSystem[] _particleSystems;

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
		Universe.ShiftableItems.Add(this);
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
		Universe.ShiftableItems.Remove(this);
	}

	public void Shift(Vector3 shiftAmount)
	{
		transform.position -= shiftAmount;

		if (OnShift != null)
			OnShift(shiftAmount);

		var particleShift = Vector3.zero - shiftAmount;
		foreach (var ps in _particleSystems)
		{
			Utility.MoveParticles(ps, particleShift);
		}
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
}