using UnityEngine;

public class PlayerInventory {

    private GameObject[] _items;

    public GameObject[] Items { get { return _items; } }

    public PlayerInventory(int inventorySize)
    {
        _items = new GameObject[inventorySize];
    }

    public void AddItem(GameObject item)
    {
        var availableIndex = 0;
        for(var i =0; i<_items.Length; i++)
        {
            if (_items[i] == null)
            {
                availableIndex = i;
                break;
            }
        }
        AddItemAt(item, availableIndex);
    }

    public void AddItemAt(GameObject item, int index)
    {
        _items[index] = item;
    }

    public void RemoveItemAt(int index)
    {
        _items[index] = null;
    }
}
