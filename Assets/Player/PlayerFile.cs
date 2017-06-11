using System.Xml.Serialization;

[XmlRoot("player")]
public class PlayerFile : DataFile<PlayerFile>
{
    [XmlElement("junk")]
    public int SpaceJunk;
}
