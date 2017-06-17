using System.Collections.Generic;
using System.Linq;

public class PlayerInventory
{
    private List<PlayerFile.InventoryItem> _items;

    public List<PlayerFile.InventoryItem> Items { get { return _items; }  }

    public PlayerInventory(PlayerFile playerFile)
    {
        if (playerFile.Inventory != null)
        {
            _items = playerFile.Inventory;
        }
        else
        {
            _items = new List<PlayerFile.InventoryItem>();
        }
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
            item.BluePrintsOwned = 1;
            _items.Add(item);
        }
        var playerFile = PlayerController.Current.BuildFile();
        playerFile.Inventory = _items;
        playerFile.WriteToFile(PlayerFile.Filename);
    }
}
