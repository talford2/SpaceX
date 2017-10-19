using UnityEditor;
using UnityEngine;

public class CreateVehicleCorpseEditor : EditorWindow
{
    private string renameTo;
    private GameObject explosionPrefab;
    private GameObject smokePrefab;
    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Corpse")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow(typeof(CreateVehicleCorpseEditor));
    }

    public void BuildCorpse()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            selected.name = renameTo;

            var rigidBody = selected.AddComponent<Rigidbody>();
            rigidBody.mass = 400f;
            rigidBody.useGravity = false;

            var killable = selected.AddComponent<Killable>();
            killable.MaxHealth = 30f;
            killable.Health = killable.MaxHealth;
            killable.DestroyOnDie = false;

            selected.AddComponent<Shiftable>();
            var corpse = selected.AddComponent<VehicleCorpse>();
            corpse.ExplosionPrefab = explosionPrefab;

            foreach (Transform transform in selected.GetComponentsInChildren<Transform>())
            {
                var transName = transform.name.ToLowerInvariant();
                if (transName.Contains("thruster"))
                {
                    var smokeInstance = Instantiate(smokePrefab, transform);
                    smokeInstance.transform.localPosition = Vector3.zero;
                    smokeInstance.transform.localRotation = Quaternion.identity;
                    smokeInstance.transform.localScale = Vector3.one;
                }
            }
        }
    }

    private void OnGUI()
    {
        var selected = Selection.activeGameObject;
        if (selected)
        {
            EditorGUILayout.PrefixLabel("Selected");
            var boldText = new GUIStyle();
            boldText.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(selected.name, boldText);
            renameTo = string.Format("{0}Corpse", selected.name);

            EditorGUILayout.PrefixLabel("Rename to");
            renameTo = EditorGUILayout.TextField(renameTo);

            EditorGUILayout.PrefixLabel("Explosion Prefab");
            explosionPrefab = (GameObject)EditorGUILayout.ObjectField(explosionPrefab, typeof(GameObject), false);
            EditorGUILayout.PrefixLabel("Smoke Prefab");
            smokePrefab = (GameObject)EditorGUILayout.ObjectField(smokePrefab, typeof(GameObject), false);
            if (!string.IsNullOrEmpty(renameTo) && explosionPrefab != null && smokePrefab != null)
            {
                if (GUILayout.Button("Create"))
                {
                    BuildCorpse();
                    windowInstance.Close();
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("Select an object to transform in to corpse.");
        }
    }
}
