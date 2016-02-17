using UnityEngine;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    public GameObject TurretPrefab;
    public List<Transform> TurretTransforms;
    public List<Transform> BayTransforms;
    public List<Killable> Killables;

    private int _liveCount;

    private void Start()
    {
        foreach (var turretTransform in TurretTransforms)
        {
            var turret = (GameObject)Instantiate(TurretPrefab, turretTransform.position, turretTransform.rotation);
            turret.transform.SetParent(turretTransform);
            Killables.Add(turret.GetComponent<Killable>());
        }
        foreach (var bayTransform in BayTransforms)
        {
            Killables.Add(bayTransform.GetComponent<Killable>());
        }
        _liveCount = Killables.Count;
        foreach (var killable in Killables)
        {
            killable.OnDie += OnKill;
        }
    }

    private void OnKill(Killable sender)
    {
        _liveCount--;
        Debug.Log("MOTHERSHIP LIVE COUNT: " + _liveCount);
        if (_liveCount <= 0)
        {
            Debug.Log("MOTHERSHIP DESTROYED!!!");
        }
    }
}