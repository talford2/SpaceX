using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateVehicleEditor : EditorWindow
{
    private string prefabName;
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
        Debug.Log("SET DEFAULTS!");
        SetDefaults();
    }

    private void SetDefaults()
    {
        primaryWeaponIndex = GetPathIndexFor("GreenLaser", weaponsPath, assetFilter);
        secondaryWeaponIndex = GetPathIndexFor("Seeker", weaponsPath, assetFilter);
        shieldIndex = GetPathIndexFor("StandardShield", "Assets/Shields");
        engineIndex = GetPathIndexFor("StandardEngine", "Assets/Engines");
        woundEffectIndex = GetPathIndexFor("Smoke", effectsPath);
        deathExplosionIndex = GetPathIndexFor("FieryExplosion", effectsPath);
        spinDeathExplosionIndex = GetPathIndexFor("FierySmallExplosion", effectsPath);
        trackerIndex = GetPathIndexFor("EnemyFighter", trackersPath, assetFilter);
    }

    private void OnGUI()
    {
        modelPrefab = FieldFor("From model", modelPrefab);
        if (modelPrefab != null)
        {
            prefabName = modelPrefab.name;

            prefabName = FieldFor("Vehicle prefab name", prefabName);

            primaryWeaponIndex = PathDropdown(weaponsPath, "Primary weapon", primaryWeaponIndex, assetFilter);
            primaryWeaponDefinition = FromPathIndex<WeaponDefinition>(weaponsPath, primaryWeaponIndex, assetFilter);

            secondaryWeaponIndex = PathDropdown(weaponsPath, "Secondary weapon", secondaryWeaponIndex, assetFilter);
            secondaryWeaponDefinition = FromPathIndex<WeaponDefinition>(weaponsPath, secondaryWeaponIndex, assetFilter);

            shieldIndex = PathDropdown(shieldsPath, "Shield", shieldIndex);
            shield = FromPathIndex<Shield>(shieldsPath, shieldIndex);

            engineIndex = PathDropdown(enginesPath, "Engine", engineIndex);
            engine = FromPathIndex<Engine>(enginesPath, engineIndex);

            woundEffectIndex = PathDropdown(effectsPath, "Wound effect", woundEffectIndex);
            woundEffect = FromPathIndex<GameObject>(effectsPath, woundEffectIndex);

            deathExplosionIndex = PathDropdown(effectsPath, "Explode death explosion", deathExplosionIndex);
            deathExplosion = FromPathIndex<GameObject>(effectsPath, deathExplosionIndex);

            spinDeathExplosionIndex = PathDropdown(effectsPath, "Spin death explosion", spinDeathExplosionIndex);
            spinDeathExplosion = FromPathIndex<GameObject>(effectsPath, spinDeathExplosionIndex);

            thrusterPrefabIndex = PathDropdown(thrustersPath, "Thruster effect", thrusterPrefabIndex);
            thrusterPrefab = FromPathIndex<GameObject>(thrustersPath, thrusterPrefabIndex);

            trackerIndex = PathDropdown(trackersPath, "Tracker values", trackerIndex, assetFilter);
            trackerValues = FromPathIndex<VehicleTrackerValues>(trackersPath, trackerIndex, assetFilter);

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

    private T FieldFor<T>(string label, T value) where T : Object
    {
        EditorGUILayout.PrefixLabel(label);
        return (T)EditorGUILayout.ObjectField(value, typeof(T), false);
    }

    private string FieldFor(string label, string value)
    {
        EditorGUILayout.PrefixLabel(label);
        return EditorGUILayout.TextField(value);
    }

    private List<string> PrefabsFromPath(string path, string filter = "*.prefab")
    {
        var assetParentPath = new DirectoryInfo(Application.dataPath).Parent;
        var prefabFiles = Directory.GetFiles(string.Format("{0}/{1}", assetParentPath, path), filter);
        var prefabNames = new List<string>();
        foreach (var file in prefabFiles)
        {
            var filename = new FileInfo(file).Name;
            var prefab = AssetDatabase.LoadAssetAtPath<Object>(string.Format("{0}/{1}", path, filename));
            prefabNames.Add(prefab.name);
        }
        return prefabNames;
    }

    private int GetPathIndexFor(string prefabName, string path, string filter="*.prefab")
    {
        var assetParentPath = new DirectoryInfo(Application.dataPath).Parent;
        var prefabFiles = Directory.GetFiles(string.Format("{0}/{1}", assetParentPath, path), filter);
        for(var i =0; i<prefabFiles.Length;i++)
        {
            var prefabFilename = new FileInfo(prefabFiles[i]).Name;
            var prefab = AssetDatabase.LoadAssetAtPath<Object>(string.Format("{0}/{1}", path, prefabFilename));
            if (prefabName.Equals(prefab.name))
                return i;
        }
        Debug.LogWarningFormat("Could not file prefab named '{0}' under '{1}' applying filter '{2}'.", prefabName, path, filter);
        return -1;
    }

    private T FromPathIndex<T>(string path, int index, string filter = "*.prefab") where T : Object
    {
        var assetParentPath = new DirectoryInfo(Application.dataPath).Parent;
        var prefabFiles = Directory.GetFiles(string.Format("{0}/{1}", assetParentPath, path), filter);
        var prefabFilename = new FileInfo(prefabFiles[index]).Name;
        return AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}/{1}", path, prefabFilename));
    }

    private int PathDropdown(string path, string label, int selectedIndex, string filter = "*.prefab")
    {
        EditorGUILayout.PrefixLabel(label);
        var prefabNames = PrefabsFromPath(path, filter).ToArray();
        return EditorGUILayout.Popup(selectedIndex, prefabNames);
    }

    private bool IsFormComplete()
    {
        return
            modelPrefab != null &&
            !string.IsNullOrEmpty(prefabName) &&
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
        vehicle.ShieldPrefab = FromPathIndex<Shield>("Assets/Shields", shieldIndex);
        vehicle.EnginePrefab = FromPathIndex<Engine>("Assets/Engines", engineIndex);
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

    /*
    public void BuildVehicle()
    {
        
        var parentObj = new GameObject(renameTo);
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            selected.transform.SetParent(parentObj.transform);
            selected.transform.localPosition = Vector3.zero;
            selected.transform.localRotation = Quaternion.identity;
            selected.transform.localScale = Vector3.one;

            var existingVehicle = parentObj.GetComponent<Vehicle>();
            var vehicle = existingVehicle == null
                ? parentObj.AddComponent<Vehicle>()
                : existingVehicle;

            List<Transform> thrusterTransforms = new List<Transform>();
            List<ShootPoint> shootPoints = new List<ShootPoint>();
            foreach (Transform transform in selected.GetComponentsInChildren<Transform>())
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
            vehicle.MeshTransform = selected.transform;
            vehicle.WoundEffectTransforms = thrusterTransforms;
            vehicle.ThrusterTransforms = thrusterTransforms;
            vehicle.PrimaryShootPoints = shootPoints;
            vehicle.SecondaryShootPoints = shootPoints;

            parentObj.AddComponent<Shiftable>();
            var killable = parentObj.AddComponent<Killable>();
            killable.MaxHealth = 50f;
            killable.Health = killable.MaxHealth;

            var rigidBody = parentObj.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;

            parentObj.AddComponent<VehicleTracker>();
            parentObj.AddComponent<Targetable>();

            var proximitySensor = parentObj.AddComponent<ProximitySensor>();
            proximitySensor.Radius = 50f;

            var shieldRegenerator = parentObj.AddComponent<ShieldRegenerator>();
            shieldRegenerator.RegenerationRate = 5f;
            shieldRegenerator.RegenerationDelay = 5f;

            parentObj.AddComponent<Shaker>();

            var detectableGo = new GameObject("Detectable");
            detectableGo.AddComponent<SphereCollider>();
            var detectable = detectableGo.AddComponent<Detectable>();
            detectable.TargetTransform = selected.transform;
            detectableGo.transform.SetParent(selected.transform);
            detectableGo.transform.localPosition = Vector3.zero;
            detectableGo.transform.localScale = Vector3.one;
            detectableGo.transform.localRotation = Quaternion.identity;
        }
        
    }
    */
}
