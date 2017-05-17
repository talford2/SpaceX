using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("level")]
public class LevelFile : DataFile<LevelFile>
{
    [XmlElement("sundir")]
    public Vector3 SunDirection;

}
