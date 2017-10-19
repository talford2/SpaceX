using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateVehiclePreviewEditor : EditorWindow
{
    private GameObject modelPrefab;
    private string prefabName;

    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Preview")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow(typeof(CreateVehiclePreviewEditor));
    }

    private void BuildPreview()
    {
        var previewObj = Instantiate(modelPrefab);
        var meshRendererObj = previewObj.GetComponentInChildren<MeshRenderer>().gameObject;
        meshRendererObj.name = prefabName;
        meshRendererObj.transform.SetParent(null);
        DestroyImmediate(previewObj);
    }

    private void OnGUI()
    {
        modelPrefab = FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = string.Format("{0}Preview", modelPrefab.name);

            if (IsFormComplete())
            {
                if (GUILayout.Button("Create"))
                {
                    BuildPreview();
                    windowInstance.Close();
                }
            }
        }
    }

    private bool IsFormComplete()
    {
        return
            modelPrefab != null &&
            !string.IsNullOrEmpty(prefabName);
    }

    private T FieldFor<T>(string label, T value) where T : Object
    {
        return (T)EditorGUILayout.ObjectField(label, value, typeof(T), false);
    }

    private string FieldFor(string label, string value)
    {
        return EditorGUILayout.TextField(label, value);
    }

    private float FieldFor(string label, float value)
    {
        return EditorGUILayout.FloatField(label, value);
    }

    private T DropdownPathFieldFor<T>(string label, string path, string filter, ref int index, T value) where T : Object
    {
        index = EditorExtensions.PathDropdown(path, label, index);
        return EditorExtensions.FromPathDropdownIndex<T>(path, index, filter);
    }
}
