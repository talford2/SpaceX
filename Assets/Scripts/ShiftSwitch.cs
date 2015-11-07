using UnityEngine;

[RequireComponent(typeof (Shiftable))]
public class ShiftSwitch : MonoBehaviour
{
    public MonoBehaviour SwitchComponent;

    private Shiftable _shiftable;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Switch;
    }

    private void Switch(CellIndex delta)
    {
        SwitchComponent.enabled = PlayerController.Current.InPlayerActiveCells(_shiftable.UniverseCellIndex);
    }
}
