using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public abstract class DataFile<T>
{
    public static T ReadFromFile(string filename)
    {
        var serializer = new XmlSerializer(typeof(T));
        Debug.Log("read: " + filename);
        using (var fileStream = new FileStream(filename, FileMode.Open))
        {
            return (T)serializer.Deserialize(fileStream);
        }
    }

    public void WriteToFile(string filename)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var fileStream = new FileStream(filename, FileMode.Create))
        {
            serializer.Serialize(fileStream, this);
        }
    }
}
