﻿using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class MapPin : MonoBehaviour
{
    public GameObject ActivePin;

    public GameObject PinInstance;

    private Shiftable _shiftable;

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        Map.Current.AddPin(this);
    }

    private void OnDestroy()
    {
        Map.Current.RemovePin(this);
    }
}
