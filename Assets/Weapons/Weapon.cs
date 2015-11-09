using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject MissilePrefab;

    public float FireRate = 0.2f;

    private float _fireCooldown = 0f;
    private Transform _shootPoint;

    public bool IsTriggered;

    public int MissilePoolCount = 20;

    public delegate void OnShootEvent();
    public OnShootEvent OnShoot;

    private int curMissileIndex;
    private List<GameObject> missileInstances;

    public void Initialize(GameObject owner)
    {
        curMissileIndex = 0;
        missileInstances = new List<GameObject>();
        var missilesContainer = Utility.FindOrCreateContainer("Missiles");
        for (var i = 0; i < MissilePoolCount; i++)
        {
            var missileInstance = Utility.InstantiateInParent(MissilePrefab, missilesContainer);
            missileInstance.GetComponent<Missile>().SetOwner(owner);
            missileInstances.Add(missileInstance);
        }
    }

    private void Update()
    {
        _fireCooldown -= Time.deltaTime;

        //Debug.LogFormat("{0} ===", IsTriggered);

        if (IsTriggered && _fireCooldown < 0)
        {
            //Debug.Log("Shoot!");
            _fireCooldown = FireRate;
            Fire();
        }
    }

    public void SetShootPoint(Transform shootPoint)
    {
        _shootPoint = shootPoint;
    }

    public void Fire()
    {
        var missile = missileInstances[curMissileIndex];

        if (OnShoot != null)
            OnShoot();

        missile.transform.position = _shootPoint.position;
        missile.transform.rotation = _shootPoint.rotation;
        missile.GetComponent<Shiftable>().UniverseCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
        missile.GetComponent<Missile>().Shoot(_shootPoint.position);

        curMissileIndex++;
        if (curMissileIndex >= missileInstances.Count)
            curMissileIndex = 0;
    }
}
