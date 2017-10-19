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
        smokeIndex = GetEffectsDropdownIndexFor("SmokeAndFire");
        explosionIndex = GetEffectsDropdownIndexFor("FieryExplosion");
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
                Utility.ResetLocalTransform(smokeInstance.transform);
                smokeInstances.Add(smokeInstance.GetComponent<ParticleSystem>());
            }
        }
        corpse.SmokeSystems = smokeInstances;
    }

    private void OnGUI()
    {
        modelPrefab = EditorExtensions.FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = string.Format("{0}Corpse", modelPrefab.name);

            mass = EditorExtensions.FieldFor("Mass", mass);

            smokePrefab = EffectsDropdownFieldFor("Smoke prefab", ref smokeIndex, smokePrefab);

            explosionPrefab = EffectsDropdownFieldFor("Explosion prefab", ref explosionIndex, explosionPrefab);

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

    private T EffectsDropdownFieldFor<T>(string label, ref int index, T value) where T: Object
    {
        return EditorExtensions.PrefabPathFieldFor(label, effectsPath, ref index, value);
    }

    private int GetEffectsDropdownIndexFor(string prefabName)
    {
        return EditorExtensions.GetPrefabPathIndexFor(effectsPath, prefabName);
    }
}
