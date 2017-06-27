using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehiclePool : MonoBehaviour {

    private static VehiclePool _current;
    public static VehiclePool Current { get { return _current; } }

    public List<Vehicle> Vehicles;

    private void Awake()
    {
        _current = this;
    }

    public static Vehicle ByKey(string key)
    {
        return _current.Vehicles.FirstOrDefault(v => v.name == key);
    }
}
