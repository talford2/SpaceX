using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateVehicleEditor : EditorWindow
{
    private string prefabName;
    private string vehicleName;
    private GameObject modelPrefab;
    private WeaponDefinition primaryWeaponDefinition;
    private int primaryWeaponIndex;
    private WeaponDefinition secondaryWeaponDefinition;
    private int secondaryWeaponIndex;

    private Shield shield;
    private int shieldIndex;
    private Engine engine;
    private int engineIndex;

    private GameObject woundEffect;
    private int woundEffectIndex;

    private GameObject deathExplosion;
    private int deathExplosionIndex;

    private GameObject spinDeathExplosion;
    private int spinDeathExplosionIndex;

    private GameObject thrusterPrefab;
    private int thrusterPrefabIndex;

    private VehicleTrackerValues trackerValues;
    private int trackerIndex;

    private string effectsPath = "Assets/Effects";
    private string weaponsPath = "Assets/Weapons/Definitions";
    private string shieldsPath = "Assets/Shields";
    private string enginesPath = "Assets/Engines";
    private string thrustersPath = "Assets/Vehicles";
    private string trackersPath = "Assets/HUD/Trackers";
    private string assetFilter = "*.asset";

    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Vehicle")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow(typeof(CreateVehicleEditor));
    }

    private void Awake()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        primaryWeaponIndex = EditorExtensions.GetPathDropdownIndexFor("GreenLaser", weaponsPath, assetFilter);
        secondaryWeaponIndex = EditorExtensions.GetPathDropdownIndexFor("Seeker", weaponsPath, assetFilter);
        shieldIndex = EditorExtensions.GetPathDropdownIndexFor("StandardShield", shieldsPath);
        engineIndex = EditorExtensions.GetPathDropdownIndexFor("StandardEngine", enginesPath);
        woundEffectIndex = EditorExtensions.GetPathDropdownIndexFor("Smoke", effectsPath);
        deathExplosionIndex = EditorExtensions.GetPathDropdownIndexFor("FieryExplosion", effectsPath);
        spinDeathExplosionIndex = EditorExtensions.GetPathDropdownIndexFor("FierySmallExplosion", effectsPath);
        trackerIndex = EditorExtensions.GetPathDropdownIndexFor("EnemyFighter", trackersPath, assetFilter);
    }

    private void OnGUI()
    {
        modelPrefab = FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = modelPrefab.name;

            prefabName = FieldFor("Vehicle prefab name", prefabName);

            vehicleName = FieldFor("Vehicle name", vehicleName);

            primaryWeaponIndex = EditorExtensions.PathDropdown(weaponsPath, "Primary weapon", primaryWeaponIndex, assetFilter);
            primaryWeaponDefinition = EditorExtensions.FromPathDropdownIndex<WeaponDefinition>(weaponsPath, primaryWeaponIndex, assetFilter);

            secondaryWeaponIndex = EditorExtensions.PathDropdown(weaponsPath, "Secondary weapon", secondaryWeaponIndex, assetFilter);
            secondaryWeaponDefinition = EditorExtensions.FromPathDropdownIndex<WeaponDefinition>(weaponsPath, secondaryWeaponIndex, assetFilter);

            shieldIndex = EditorExtensions.PathDropdown(shieldsPath, "Shield", shieldIndex);
            shield = EditorExtensions.FromPathDropdownIndex<Shield>(shieldsPath, shieldIndex);

            engineIndex = EditorExtensions.PathDropdown(enginesPath, "Engine", engineIndex);
            engine = EditorExtensions.FromPathDropdownIndex<Engine>(enginesPath, engineIndex);

            woundEffectIndex = EditorExtensions.PathDropdown(effectsPath, "Wound effect", woundEffectIndex);
            woundEffect = EditorExtensions.FromPathDropdownIndex<GameObject>(effectsPath, woundEffectIndex);

            deathExplosionIndex = EditorExtensions.PathDropdown(effectsPath, "Explode death explosion", deathExplosionIndex);
            deathExplosion = EditorExtensions.FromPathDropdownIndex<GameObject>(effectsPath, deathExplosionIndex);

            spinDeathExplosionIndex = EditorExtensions.PathDropdown(effectsPath, "Spin death explosion", spinDeathExplosionIndex);
            spinDeathExplosion = EditorExtensions.FromPathDropdownIndex<GameObject>(effectsPath, spinDeathExplosionIndex);

            thrusterPrefabIndex = EditorExtensions.PathDropdown(thrustersPath, "Thruster effect", thrusterPrefabIndex);
            thrusterPrefab = EditorExtensions.FromPathDropdownIndex<GameObject>(thrustersPath, thrusterPrefabIndex);

            trackerIndex = EditorExtensions.PathDropdown(trackersPath, "Tracker values", trackerIndex, assetFilter);
            trackerValues = EditorExtensions.FromPathDropdownIndex<VehicleTrackerValues>(trackersPath, trackerIndex, assetFilter);

            if (IsFormComplete())
            {
                if (GUILayout.Button("Create"))
                {
                    BuildVehicle();
                    windowInstance.Close();
                }
            }
        }
    }

    private bool IsFormComplete()
    {
        return
            modelPrefab != null &&
            !string.IsNullOrEmpty(prefabName) &&
            !string.IsNullOrEmpty(vehicleName) &&
            primaryWeaponDefinition != null &&
            secondaryWeaponDefinition != null &&
            shield != null &&
            engine != null &&
            woundEffect != null &&
            deathExplosion != null &&
            spinDeathExplosion != null &&
            trackerValues != null;
    }

    private void ResetLocalTransform(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    private void BuildVehicle()
    {
        var vehicleObj = new GameObject(prefabName);
        var modelInstance = Instantiate(modelPrefab, vehicleObj.transform);
        modelInstance.name = modelPrefab.name;
        ResetLocalTransform(modelInstance.transform);

        // Vehicle
        var vehicle = vehicleObj.AddComponent<Vehicle>();
        vehicle.MeshTransform = modelInstance.transform;
        vehicle.Name = vehicleName;

        List<Transform> thrusterTransforms = new List<Transform>();
        List<ShootPoint> shootPoints = new List<ShootPoint>();
        foreach (Transform transform in modelInstance.GetComponentsInChildren<Transform>())
        {
            var transName = transform.name.ToLowerInvariant();
            if (transName.Contains("thruster"))
            {
                thrusterTransforms.Add(transform);
            }
            if (transName.Contains("shootpoint"))
            {
                shootPoints.Add(transform.gameObject.AddComponent<ShootPoint>());
            }
        }
        vehicle.WoundEffectTransforms = thrusterTransforms;
        vehicle.ThrusterTransforms = thrusterTransforms;
        vehicle.PrimaryShootPoints = shootPoints;
        vehicle.SecondaryShootPoints = shootPoints;
        vehicle.PrimaryWeapon = primaryWeaponDefinition;
        vehicle.SecondaryWeapon = secondaryWeaponDefinition;
        vehicle.ShieldPrefab = shield;
        vehicle.EnginePrefab = engine;
        vehicle.WoundEffect = woundEffect;
        vehicle.ShieldShakeAmplitude = 0.1f;
        vehicle.ExplosiveForce = 50f;
        vehicle.ExplodeDeathExplosion = deathExplosion;
        vehicle.SpinDeathExplosion = spinDeathExplosion;
        vehicle.ThrusterPrefab = thrusterPrefab;

        // Shiftable
        vehicleObj.AddComponent<Shiftable>();

        // Killable
        var killable = vehicleObj.AddComponent<Killable>();
        killable.MaxHealth = 50f;
        killable.Health = killable.MaxHealth;

        // Rigid Body
        var rigidBody = vehicleObj.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;

        // Tracker
        var tracker = vehicleObj.AddComponent<VehicleTracker>();
        tracker.Options = trackerValues;

        // Targetable
        vehicleObj.AddComponent<Targetable>();

        // Proximity Sensor
        var proximitySensor = vehicleObj.AddComponent<ProximitySensor>();
        proximitySensor.Radius = 50f;

        // Shield regenerator
        var shieldRegenerator = vehicleObj.AddComponent<ShieldRegenerator>();
        shieldRegenerator.RegenerationRate = 5f;
        shieldRegenerator.RegenerationDelay = 5f;

        // Shaker
        vehicleObj.AddComponent<Shaker>();

        // Detectable
        var detectableGo = new GameObject("Detectable");
        detectableGo.AddComponent<SphereCollider>();
        var detectable = detectableGo.AddComponent<Detectable>();
        detectable.TargetTransform = vehicleObj.transform;
        detectableGo.transform.SetParent(modelInstance.transform);
        ResetLocalTransform(detectableGo.transform);
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
