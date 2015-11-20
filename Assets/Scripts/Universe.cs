using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public float CellSize = 200f;

    public float HalfCellSize { get; private set; }

    public Shiftable PlayerSpawnPosition;

    public UniverseCamera ViewPort;

    public static List<Shiftable> ShiftableItems;

    private static Universe _current;

    public static Universe Current
    {
        get { return _current; }
    }

    public void Awake()
    {
        ShiftableItems = new List<Shiftable>();
        _current = this;
        HalfCellSize = CellSize/2f;
    }

    public void Start()
    {
        Debug.Log("Universe start");

        //// Move the player to the start position
        if (PlayerController.Current != null)
        {
            WarpTo(PlayerSpawnPosition);
        }
        ViewPort.Shiftable.OnCellIndexChange += Shift;
    }

    public void Shift(CellIndex delta)
    {
        foreach (var shiftable in ShiftableItems)
        {
            shiftable.Shift(delta.ToVector3()*CellSize);
        }
    }

    public void WarpTo(Shiftable spawner)
    {
        WarpTo(spawner.UniversePosition);
    }

    public void WarpTo(UniversePosition universePosition)
    {
        ViewPort.Shiftable.SetShiftPosition(universePosition);
        var shiftDelta = ViewPort.Shiftable.UniverseCellIndex - universePosition.CellIndex;
        Shift(shiftDelta);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one*CellSize);
    }

    private void OnGUI()
    {

        var cellIndex = ViewPort.Shiftable.UniverseCellIndex;
        GUI.Label(new Rect(50f, 50f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", cellIndex.X, cellIndex.Y, cellIndex.Z));
        /*
        GUI.Label(new Rect(50f, Screen.height - 50f, 100f, 20f), string.Format("ENERGY: {0:f1}", PlayerController.Current.VehicleInstance.BoostEnergy));
        */
    }

    public Vector3 GetWorldPosition(UniversePosition universePosition)
    {
        return GetWorldPosition(universePosition.CellIndex, universePosition.CellLocalPosition);
    }

    public UniversePosition GetUniversePosition(Vector3 worldPosition)
    {
        return new UniversePosition(CellIndexFromWorldPosition(worldPosition), CellLocalPositionFromWorldPosition(worldPosition));
    }

    private Vector3 GetWorldPosition(CellIndex cellIndex, Vector3 positionInCell)
    {
        var dCell = cellIndex - ViewPort.Shiftable.UniverseCellIndex;
        return dCell.ToVector3()*Current.CellSize + positionInCell;
    }

    private CellIndex CellIndexFromWorldPosition(Vector3 worldPosition)
    {
        var dCell = (worldPosition - Vector3.one*HalfCellSize)/CellSize;
        return new CellIndex(Mathf.CeilToInt(dCell.x), Mathf.CeilToInt(dCell.y), Mathf.CeilToInt(dCell.z)) + ViewPort.Shiftable.UniverseCellIndex;
    }

    private Vector3 CellLocalPositionFromWorldPosition(Vector3 worldPosition)
    {
        var cell = CellIndexFromWorldPosition(worldPosition);
        return worldPosition - cell.ToVector3()*CellSize + ViewPort.Shiftable.UniverseCellIndex.ToVector3()*CellSize;
    }    
}