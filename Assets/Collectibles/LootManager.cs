using UnityEngine;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    public List<GameObject> Items;

    private static LootManager _current;
    public static LootManager Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i].GetComponent<Weapon>();
            if (item != null)
                item.LootIndex = i;
        }
    }
}
