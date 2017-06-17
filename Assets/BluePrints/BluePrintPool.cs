using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BluePrintPool : MonoBehaviour
{
    private static BluePrintPool _current;

    public List<BluePrint> BluePrints;

    private void Awake()
    {
        _current = this;
    }

    public static BluePrint ByKey(string key)
    {
        return _current.BluePrints.FirstOrDefault(w => w.Key == key);
    }

    public static List<BluePrint> All()
    {
        return _current.BluePrints;
    }
}
