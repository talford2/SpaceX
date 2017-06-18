using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public abstract class DataFile<T>
{
    protected static bool Exists(string filename)
    {
        return File.Exists(filename);
    }

    public static T ReadFromFile(string filename)
    {
        var serializer = new XmlSerializer(typeof(T));
        Debug.Log("read: " + filename);
        using (StreamReader stream = new StreamReader(filename, Encoding.GetEncoding("UTF-8")))
        {
            return (T)serializer.Deserialize(stream);
        }
    }

    public void WriteToFile(string filename)
    {
        var serializer = new XmlSerializer(typeof(T));
        Debug.Log("write: " + filename);
        using (StreamWriter stream = new StreamWriter(filename, false, Encoding.GetEncoding("UTF-8")))
        {
            serializer.Serialize(stream, this);
        }
    }
}
