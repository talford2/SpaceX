using System.Collections.Generic;
using UnityEngine;

public class PirateSquad : UniverseEvent
{
    private Material _testMat;
    private MapPin _mapPin;

    public Renderer Sphere;

    public List<Spawner> Spawners;

    public override void Awake()
    {
        base.Awake();
        _testMat = Sphere.material;
        _testMat.SetColor("_Color", Color.yellow);
        _mapPin = GetComponent<MapPin>();
    }

    public override void Initialise()
    {
        foreach (var spawner in Spawners)
        {
            spawner.Spawn();
        }
        _testMat.SetColor("_Color", Color.red);
        if (_mapPin != null)
            _mapPin.SetPinState(MapPin.MapPinState.Inactive);
        base.Initialise();
    }
}
