using UnityEngine;

public class Shiftable : MonoBehaviour
{
    [Tooltip("Cell the object is contained in")]
    public CellIndex UniverseCellIndex;

    public delegate void OnShiftEvent(CellIndex delta);
    public event OnShiftEvent OnShift;

    private void Awake()
    {
        Universe.ShiftableItems.Add(this);
    }

    private void Update()
    {
        var curCellZero = Universe.Current.CellSize*UniverseCellIndex.ToVector3();
        var delta = (transform.position - curCellZero - (Vector3.one * Universe.Current.CellSize * 0.5f)) / (Universe.Current.CellSize);
        var deltaCell = new CellIndex(Mathf.CeilToInt(delta.x), Mathf.CeilToInt(delta.y), Mathf.CeilToInt(delta.z));

        Debug.Log("DELTACELL: " + deltaCell);

        if (deltaCell.X != 0 || deltaCell.Y != 0 || deltaCell.Z != 0)
        {
            Debug.Log("SHIFT!!!!!");
            UniverseCellIndex += deltaCell;

            if (OnShift != null)
            {
                Debug.Log("EXECUTE ASSIGNED EVENT!");
                OnShift(deltaCell);
            }
        }
    }

    private void OnDestroy()
    {
        Universe.ShiftableItems.Remove(this);
    }
}