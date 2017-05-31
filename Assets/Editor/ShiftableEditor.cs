using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shiftable))]
public class ShiftableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var shiftable = (Shiftable)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Calculate UniversePosition"))
        {
            var universePosition = Universe.GetUniversePositionFromZero(shiftable.transform.position, 1000f);
            shiftable.SetShiftPosition(universePosition);
        }
    }
}
