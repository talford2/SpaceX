﻿using UnityEngine;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    public GameObject TurretPrefab;
    public List<Transform> TurretTransforms;
    public List<Transform> BayTransforms;
    public MapPin MapPin;

    private int _liveCount;
    private List<Killable> _killables;

    private void Start()
    {
        _killables = new List<Killable>();
        foreach (var turretTransform in TurretTransforms)
        {
            var turret = (GameObject)Instantiate(TurretPrefab, turretTransform.position, turretTransform.rotation);
            turret.transform.SetParent(turretTransform);
            _killables.Add(turret.GetComponent<Killable>());
        }
        foreach (var bayTransform in BayTransforms)
        {
            _killables.Add(bayTransform.GetComponent<Killable>());
        }
        _liveCount = _killables.Count;
        foreach (var killable in _killables)
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
            MapPin.SetPinState(MapPin.MapPinState.Inactive);
        }
    }
}