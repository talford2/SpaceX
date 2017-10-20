using System.Collections.Generic;
using UnityEngine;

public class VehicleDebris : MonoBehaviour
{
    public List<Transform> SmokePoints;
    public GameObject SmokePrefab;

    private void Awake()
    {
        foreach (var smokePoint in SmokePoints)
        {
            var smokeInstance = Instantiate(SmokePrefab, smokePoint);
            Utility.ResetLocalTransform(smokeInstance.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var smokePoint in SmokePoints)
        {
            if (smokePoint != null)
            {
                Gizmos.DrawSphere(smokePoint.transform.position, 0.1f);
                Gizmos.DrawWireSphere(smokePoint.transform.position, 0.1f);
                GizmoExtensions.AxesPoint(smokePoint.transform.position);
            }
        }
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb != null)
            {
                Gizmos.color = Color.yellow;
                GizmoExtensions.AxesPoint(rb.transform.position + rb.centerOfMass);
            }
        }
    }
}