using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Shiftable))]
public class ShiftableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var shiftable = (Shiftable)target;

        shiftable.UseWorldPosition = EditorGUILayout.Toggle("Use World Position", shiftable.UseWorldPosition);

        EditorGUI.BeginDisabledGroup(shiftable.UseWorldPosition);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Universe Cell Index");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("X");
        shiftable.UniverseCellIndex.X = EditorGUILayout.IntField(shiftable.UniverseCellIndex.X);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Y");
        shiftable.UniverseCellIndex.Y = EditorGUILayout.IntField(shiftable.UniverseCellIndex.Y);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Z");
        shiftable.UniverseCellIndex.Z = EditorGUILayout.IntField(shiftable.UniverseCellIndex.Z);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        shiftable.CellLocalPosition = EditorGUILayout.Vector3Field("Cell Local Position", shiftable.CellLocalPosition);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Calculate UniversePosition"))
        {
            var universePosition = Universe.GetUniversePositionFromZero(shiftable.transform.position, 1000f);
            shiftable.SetShiftPosition(universePosition);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }
}
