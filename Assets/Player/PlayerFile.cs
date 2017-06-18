using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("player")]
public class PlayerFile : DataFile<PlayerFile>
{
    public static string Filename { get { return string.Format("{0}/{1}", Application.persistentDataPath, "player.xml"); } }

    [XmlElement("junk")]
    public int SpaceJunk;

    [XmlElement("primary")]
    public string PrimaryWeaponKey;

    [XmlElement("secondary")]
    public string SecondaryWeaponKey;

    [XmlArray("inventory")]
    [XmlArrayItem("item")]
    public List<InventoryItem> Inventory;

    public class InventoryItem
    {
        [XmlAttribute("key")]
        public string Key;
        [XmlElement("blueprints")]
        public int BluePrintsOwned;
        [XmlElement("owned")]
        public bool IsOwned;
    }
}
