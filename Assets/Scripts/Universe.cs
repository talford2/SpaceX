using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
	public float CellSize = 200f;

	public float HalfCellSize { get; private set; }

	public Shiftable PlayerSpawnPosition;

	public UniverseCamera ViewPort;

	public List<Shiftable> ShiftableItems;

	private static Universe _current;

	private List<UniverseEvent> _universeEvents;

	public static Universe Current
	{
		get { return _current; }
	}

	public List<UniverseEvent> UniverseEvents
	{
		get
		{
			if (_universeEvents == null)
				_universeEvents = new List<UniverseEvent>();
			return _universeEvents;
		}
	}

	public void Awake()
	{
		ShiftableItems = new List<Shiftable>();
		_current = this;
		HalfCellSize = CellSize / 2f;
	}

	public void Start()
	{
		Debug.Log("Universe start");

		//// Move the player to the start position
		if (PlayerController.Current != null)
		{
			WarpTo(PlayerSpawnPosition);
		}
		ViewPort.Shiftable.OnCellIndexChange += Shiftable_OnCellIndexChange;
	}

	private void Shiftable_OnCellIndexChange(Shiftable sender, CellIndex delta)
	{
		Shift(delta);
	}

	public void Shift(CellIndex delta)
	{
		for (var i = 0; i < ShiftableItems.Count; i++)
		{
			ShiftableItems[i].Shift(delta.ToVector3() * CellSize);
		}
	}

	public void WarpTo(Shiftable spawner)
	{
		WarpTo(spawner.UniversePosition);
	}

	public void WarpTo(UniversePosition universePosition)
	{
		dCell = universePosition.CellIndex - ViewPort.Shiftable.UniverseCellIndex;
		ViewPort.Shiftable.SetShiftPosition(universePosition);
		Shift(dCell);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one * CellSize);
	}

	public Vector3 GetWorldPosition(UniversePosition universePosition)
	{
		return GetWorldPosition(universePosition.CellIndex, universePosition.CellLocalPosition);
	}

	public UniversePosition GetUniversePosition(Vector3 worldPosition)
	{
		return new UniversePosition(CellIndexFromWorldPosition(worldPosition), CellLocalPositionFromWorldPosition(worldPosition));
	}

	public Vector3 GetAbsoluteUniversePosition(UniversePosition universePosition)
	{
		dCell = universePosition.CellIndex;
		return dCell.ToVector3() * CellSize + universePosition.CellLocalPosition;
	}

	private CellIndex dCell;
	private Vector3 GetWorldPosition(CellIndex cellIndex, Vector3 positionInCell)
	{
		dCell = cellIndex - ViewPort.Shiftable.UniverseCellIndex;
		return dCell.ToVector3() * CellSize + positionInCell;
	}

	private Vector3 dCellV;
	private CellIndex CellIndexFromWorldPosition(Vector3 worldPosition)
	{
		dCellV = (worldPosition - Vector3.one * HalfCellSize) / CellSize;
		return new CellIndex(Mathf.CeilToInt(dCellV.x), Mathf.CeilToInt(dCellV.y), Mathf.CeilToInt(dCellV.z)) + ViewPort.Shiftable.UniverseCellIndex;
		//return dCell.Set(Mathf.CeilToInt(dCellV.x), Mathf.CeilToInt(dCellV.y), Mathf.CeilToInt(dCellV.z)) + ViewPort.Shiftable.UniverseCellIndex;
	}

	private Vector3 CellLocalPositionFromWorldPosition(Vector3 worldPosition)
	{
		dCell = CellIndexFromWorldPosition(worldPosition);
		return worldPosition - dCell.ToVector3() * CellSize + ViewPort.Shiftable.UniverseCellIndex.ToVector3() * CellSize;
	}
}