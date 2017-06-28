﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("player")]
public class PlayerFile : DataFile<PlayerFile>
{
    public static string Filename { get { return string.Format("{0}/{1}", Application.persistentDataPath, "player.xml"); } }

    [XmlElement("ship")]
    public string Ship;

    [XmlElement("junk")]
    public int SpaceJunk;

    [XmlArray("ships")]
    [XmlArrayItem("ship")]
    public List<ShipItem> Ships;

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

    public class ShipItem
    {
        [XmlAttribute("key")]
        public string Key;
        [XmlElement("blueprints")]
        public int BluePrintsOwned;
        [XmlElement("owned")]
        public bool IsOwned;
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

        public int SortStatus()
        {
            if (BluePrintPool.ByKey(Key).RequiredCount > BluePrintsOwned)
                return 2;
            if (!IsOwned)
                return 1;
            return 0;
        }
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
