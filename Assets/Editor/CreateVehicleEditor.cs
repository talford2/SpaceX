using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateVehicleEditor : Editor
{
    public GameObject prefabModel = null;

    [MenuItem("Space X/Build Vehicle")]
    public static void BuildVehicle()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var existingVehicle = selected.GetComponent<Vehicle>();
            var vehicle = existingVehicle == null
                ? selected.AddComponent<Vehicle>()
                : existingVehicle;

            Transform meshTransform = null;
            List<Transform> thrusterTransforms = new List<Transform>();
            List<ShootPoint> shootPoints = new List<ShootPoint>();
            foreach(Transform transform in selected.GetComponentsInChildren<Transform>())
            {
                var transName = transform.name.ToLowerInvariant();
                if (transName.EndsWith("mesh"))
                {
                    if (meshTransform == null)
                    {
                        meshTransform = transform;
                    }
                    else
                    {
                        Debug.LogWarning("More than one mesh transform.");
                    }
                }
                if (transName.Contains("thruster"))
                {
                    thrusterTransforms.Add(transform);
                }
                if (transName.Contains("shootpoint"))
                {
                    shootPoints.Add(transform.gameObject.AddComponent<ShootPoint>());
                }
            }
            vehicle.MeshTransform = meshTransform;
            vehicle.WoundEffectTransforms = thrusterTransforms;
            vehicle.ThrusterTransforms = thrusterTransforms;
            vehicle.PrimaryShootPoints = shootPoints;
            vehicle.SecondaryShootPoints = shootPoints;

            selected.AddComponent<Shiftable>();
            var killable = selected.AddComponent<Killable>();
            killable.MaxHealth = 50f;
            killable.Health = killable.MaxHealth;

            var rigidBody = selected.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.useGravity = false;

            selected.AddComponent<VehicleTracker>();
            selected.AddComponent<Targetable>();

            var proximitySensor = selected.AddComponent<ProximitySensor>();
            proximitySensor.Radius = 50f;

            var shieldRegenerator = selected.AddComponent<ShieldRegenerator>();
            shieldRegenerator.RegenerationRate = 5f;
            shieldRegenerator.RegenerationDelay = 5f;

            selected.AddComponent<Shaker>();

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
}
