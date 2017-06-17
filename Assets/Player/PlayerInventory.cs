using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory
{
    /*
    private GameObject[] _items;

    public GameObject[] Items { get { return _items; } }

    public PlayerInventory(int inventorySize)
    {
        _items = new GameObject[inventorySize];
    }

    public void AddItem(GameObject item)
    {
        var availableIndex = 0;
        for (var i = 0; i < _items.Length; i++)
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
    */

    private List<PlayerFile.InventoryItem> _items;

    public List<PlayerFile.InventoryItem> Items { get { return _items; }  }

    public PlayerInventory(PlayerFile playerFile)
    {
        _items = playerFile.Inventory;
    }

    public void AddItem(PlayerFile.InventoryItem item)
    {
        if (_items.Any(i => i.Key == item.Key))
        {
            var existingItem = _items.First(i => i.Key == item.Key);
            existingItem.BluePrintsOwned++;
        }
        else
        {
            _items.Add(item);
        }
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        playerFile.Inventory = _items;
        playerFile.WriteToFile(PlayerFile.Filename);
    }
}
