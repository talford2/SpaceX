using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetToMesh))]
public class ResetToMeshEditor : Editor {

    public override void OnInspectorGUI()
    {
        var resetToMesh = (ResetToMesh)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Match Transforms"))
        {
            Utility.MatchToMesh(resetToMesh.Instance, resetToMesh.PrefabMesh);
        }
    }
}
