using UnityEngine;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    public GameObject TurretPrefab;
    public List<Transform> TurretTransforms;

    private void Start()
    {
        foreach (var turretTransform in TurretTransforms)
        {
            var turret = (GameObject)Instantiate(TurretPrefab, turretTransform.position, turretTransform.rotation);
            turret.transform.SetParent(turretTransform);
        }
    }
}