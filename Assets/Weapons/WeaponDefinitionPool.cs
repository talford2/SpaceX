using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponDefinitionPool : MonoBehaviour
{
    private static WeaponDefinitionPool _current;

    public List<WeaponDefinition> Weapons;

    private void Awake()
    {
        _current = this;
    }

    public static WeaponDefinition ByKey(string key)
    {
        return _current.Weapons.FirstOrDefault(w => w.Key == key);
    }
}
