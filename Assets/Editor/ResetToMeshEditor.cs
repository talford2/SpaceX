using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetToMesh))]
public class ResetToMeshEditor : Editor {

    public override void OnInspectorGUI()
    {
        var resetToMesh = (ResetToMesh)target;
        //base.OnInspectorGUI();

        resetToMesh.Instance = (GameObject)EditorGUILayout.ObjectField("Instance", resetToMesh.Instance, typeof(GameObject), false);
        resetToMesh.PrefabMesh = (GameObject)EditorGUILayout.ObjectField("Prefab Mesh", resetToMesh.Instance, typeof(GameObject), false);
        if (GUILayout.Button("Match Transforms"))
        {
            Utility.MatchToMesh(resetToMesh.Instance, resetToMesh.PrefabMesh);
        }
    }
}
