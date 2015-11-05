using UnityEngine;

public class Shiftable : MonoBehaviour
{
    [Tooltip("Cell the object is contained in")]
    public CellIndex UniverseCellIndex;

    private void Awake()
    {
        Universe.ShiftableItems.Add(this);
    }

    private void OnDestroy()
    {
        Universe.ShiftableItems.Remove(this);
    }
}