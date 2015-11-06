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

    public void UpdatePosition()
    {
        var delta = (transform.position - lastPosition);
        Debug.Log("DELTA: " + delta);
        Debug.Log("CELLSIZE: " + Universe.Current.CellSize);
        var deltaCell = new CellIndex(Mathf.FloorToInt(delta.x / Universe.Current.CellSize), Mathf.FloorToInt(delta.y / Universe.Current.CellSize), Mathf.FloorToInt(delta.z / Universe.Current.CellSize));

        Debug.Log("DELTACELL: " + deltaCell);

        if (!deltaCell.IsZero())
        {
            //Debug.Log("SHIFT!!!!!");
            UniverseCellIndex += deltaCell;

            if (OnShift != null)
            {
                //Debug.Log("EXECUTE ASSIGNED EVENT!");
                OnShift(deltaCell);
            }
        }
    }

    public void UpdateLastPosition(Vector3 position)
    {
        lastPosition = position;
    }

    private void OnDestroy()
    {
        Universe.ShiftableItems.Remove(this);
    }
}