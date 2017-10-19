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

    private static EditorWindow windowInstance;

    [MenuItem("Space X/Vehicles/Build Vehicle", priority = 0)]
    public static void ShowWindow()
    {
        windowInstance = GetWindow<CreateVehicleEditor>();
    }

    private void Awake()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        primaryWeaponIndex = EditorExtensions.GetAssetPathIndexFor(weaponsPath, "GreenLaser");
        secondaryWeaponIndex = EditorExtensions.GetAssetPathIndexFor(weaponsPath, "Seeker");
        shieldIndex = EditorExtensions.GetPrefabPathIndexFor(shieldsPath, "StandardShield");
        engineIndex = EditorExtensions.GetPrefabPathIndexFor(enginesPath, "StandardEngine");
        woundEffectIndex = GetEffectsDropdownIndexFor("Smoke");
        deathExplosionIndex = GetEffectsDropdownIndexFor("FieryExplosion");
        spinDeathExplosionIndex = GetEffectsDropdownIndexFor("FierySmallExplosion");
        trackerIndex = EditorExtensions.GetAssetPathIndexFor(trackersPath, "EnemyFighter");
    }

    private void OnGUI()
    {
        modelPrefab = EditorExtensions.FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = modelPrefab.name;

            prefabName = EditorExtensions.FieldFor("Vehicle prefab name", prefabName);

            vehicleName = EditorExtensions.FieldFor("Vehicle name", vehicleName);

            primaryWeaponDefinition = EditorExtensions.AssetPathFieldFor("Primary weapon", weaponsPath, ref primaryWeaponIndex, primaryWeaponDefinition);

            secondaryWeaponDefinition = EditorExtensions.AssetPathFieldFor("Secondary weapon", weaponsPath, ref secondaryWeaponIndex, secondaryWeaponDefinition);

            shield = EditorExtensions.PrefabPathFieldFor("Shield", shieldsPath, ref shieldIndex, shield);

            engine = EditorExtensions.PrefabPathFieldFor("Engine", enginesPath, ref engineIndex, engine);

            woundEffect = EffectsDropdownFieldFor("Wound effect", ref woundEffectIndex, woundEffect);

            deathExplosion = EffectsDropdownFieldFor("Explode death explosion", ref deathExplosionIndex, deathExplosion);

            spinDeathExplosion = EffectsDropdownFieldFor("Spin death explosion", ref spinDeathExplosionIndex, spinDeathExplosion);

            thrusterPrefab = EditorExtensions.PrefabPathFieldFor("Thruster effect", thrustersPath, ref thrusterPrefabIndex, thrusterPrefab);

            trackerValues = EditorExtensions.AssetPathFieldFor("Tracker values", trackersPath, ref trackerIndex, trackerValues);

            if (IsFormComplete())
            {
                if (GUILayout.Button("Create"))
                {
                    BuildVehicle();
                    if (windowInstance == null)
                        windowInstance = GetWindow<CreateVehicleEditor>();
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

    private void BuildVehicle()
    {
        var vehicleObj = new GameObject(prefabName);
        var modelInstance = Instantiate(modelPrefab, vehicleObj.transform);
        modelInstance.name = modelPrefab.name;
        Utility.ResetLocalTransform(modelInstance.transform);

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
        Utility.ResetLocalTransform(detectableGo.transform);
    }

    private T EffectsDropdownFieldFor<T>(string label, ref int index, T value) where T : Object
    {
        return EditorExtensions.PrefabPathFieldFor(label, effectsPath, ref index, value);
    }

    private int GetEffectsDropdownIndexFor(string prefabName)
    {
        return EditorExtensions.GetPrefabPathIndexFor(effectsPath, prefabName);
    }
}
