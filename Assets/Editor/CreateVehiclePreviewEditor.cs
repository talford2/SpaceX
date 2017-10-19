using UnityEditor;
using UnityEngine;

public class CreateVehiclePreviewEditor : EditorWindow
{
    private GameObject modelPrefab;
    private string prefabName;

    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Preview", priority = 2)]
    public static void ShowWindow()
    {
        windowInstance = GetWindow<CreateVehiclePreviewEditor>();
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
        modelPrefab = EditorExtensions.FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = string.Format("{0}Preview", modelPrefab.name);

            if (IsFormComplete())
            {
                if (GUILayout.Button("Create"))
                {
                    BuildPreview();
                    if (windowInstance == null)
                        windowInstance = GetWindow<CreateVehiclePreviewEditor>();
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
}
