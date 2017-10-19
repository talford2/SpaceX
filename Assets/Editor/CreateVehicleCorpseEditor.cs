using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateVehicleCorpseEditor : EditorWindow
{
    private GameObject modelPrefab;

    private string prefabName;
    private float mass;
    private GameObject explosionPrefab;
    private int explosionIndex;
    private GameObject smokePrefab;
    private int smokeIndex;

    private string effectsPath = "Assets/Effects";
    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Corpse")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow(typeof(CreateVehicleCorpseEditor));
    }

    private void Awake()
    {
        SetDefaults();
        Debug.Log("YAY");
    }

    private void SetDefaults()
    {
        mass = 400f;
        smokeIndex = EditorExtensions.GetPathDropdownIndexFor("SmokeAndFire", effectsPath);
        explosionIndex = EditorExtensions.GetPathDropdownIndexFor("FieryExplosion", effectsPath);
    }

    public void BuildCorpse()
    {
        var corpseObj = Instantiate(modelPrefab);
        corpseObj.name = prefabName;

        var rigidBody = corpseObj.AddComponent<Rigidbody>();
        rigidBody.mass = mass;
        rigidBody.useGravity = false;

        var killable = corpseObj.AddComponent<Killable>();
        killable.MaxHealth = 30f;
        killable.Health = killable.MaxHealth;
        killable.DestroyOnDie = false;

        corpseObj.AddComponent<Shiftable>();
        var corpse = corpseObj.AddComponent<VehicleCorpse>();
        corpse.ExplosionPrefab = explosionPrefab;

        var smokeInstances = new List<ParticleSystem>();
        foreach (Transform transform in corpseObj.GetComponentsInChildren<Transform>())
        {
            var transName = transform.name.ToLowerInvariant();
            if (transName.Contains("thruster"))
            {
                var smokeInstance = Instantiate(smokePrefab, transform);
                smokeInstance.name = smokePrefab.name;
                smokeInstance.transform.localPosition = Vector3.zero;
                smokeInstance.transform.localRotation = Quaternion.identity;
                smokeInstance.transform.localScale = Vector3.one;
                smokeInstances.Add(smokeInstance.GetComponent<ParticleSystem>());
            }
        }
        corpse.SmokeSystems = smokeInstances;
    }

    private void OnGUI()
    {
        modelPrefab = FieldFor<GameObject>("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = string.Format("{0}Corpse", modelPrefab.name);

            mass = FieldFor("Mass", mass);

            smokePrefab = DropdownPathFieldFor("Smoke prefab", effectsPath, "*.prefab", ref smokeIndex, smokePrefab);

            explosionPrefab = DropdownPathFieldFor("Explosion prefab", effectsPath, "*.prefab", ref explosionIndex, explosionPrefab);

            if (IsFormComplete())
            {
                if (GUILayout.Button("Create"))
                {
                    BuildCorpse();
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
