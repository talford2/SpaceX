using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("player")]
public class PlayerFile : DataFile<PlayerFile>
{
    public static string Filename { get { return string.Format("{0}/{1}", Application.persistentDataPath, "player.xml"); } }

    [XmlElement("junk")]
    public int SpaceJunk;

    [XmlArray("inventory")]
    [XmlArrayItem("item")]
    public List<InventoryItem> Inventory;

    public InventoryItem GetItemIn(EquippedSlot equippedSlot)
    {
        return Inventory.First(i => i.EquippedSlot == equippedSlot);
    }

    public InventoryItem GetItemByKey(string key)
    {
        return Inventory.First(i => i.Key == key);
    }

    public class InventoryItem
    {
        [XmlAttribute("key")]
        public string Key;
        [XmlAttribute("equipped")]
        public EquippedSlot EquippedSlot;
        [XmlElement("blueprints")]
        public int BluePrintsOwned;
        [XmlElement("owned")]
        public bool IsOwned;
    }

    public enum EquippedSlot
    {
        Primary,
        Secondary,
        Inventory
    }

    public static bool Exists()
    {
        return Exists(Filename);
    }
}
