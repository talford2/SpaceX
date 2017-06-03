using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResetModel : EditorWindow
{
    public GameObject prefabModel = null;

    [MenuItem("Space X/Reset To Prefab")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ResetModel));
    }

    void OnGUI()
    {
        prefabModel = (GameObject)EditorGUILayout.ObjectField(prefabModel, typeof(GameObject), false);
        var selectObject = Selection.activeGameObject;
        if (prefabModel && selectObject)
        {
            if (GUILayout.Button("Reset"))
            {
                Utility.MatchToMesh(selectObject, prefabModel);
                Debug.Log("Reset : " + prefabModel.name + " => " + selectObject.name);
            }
        }
    }
}
