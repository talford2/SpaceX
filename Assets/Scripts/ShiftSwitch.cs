using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Shiftable))]
public class ShiftSwitch : MonoBehaviour
{
    public List<MonoBehaviour> SwitchOn;
    public List<MonoBehaviour> SwitchOff;

    private Shiftable _shiftable;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Switch;
    }

    private void Switch(CellIndex delta)
    {
        foreach (var onComponent in SwitchOn)
        {
            onComponent.enabled = PlayerController.Current.InPlayerActiveCells(_shiftable.UniverseCellIndex);
        }
        foreach (var offComponent in SwitchOff)
        {
            offComponent.enabled = !PlayerController.Current.InPlayerActiveCells(_shiftable.UniverseCellIndex);
        }
    }
}
