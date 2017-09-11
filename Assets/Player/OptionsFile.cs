﻿using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("options")]
public class OptionsFile : DataFile<OptionsFile>
{
    public static string Filename { get { return string.Format("{0}/{1}", Application.persistentDataPath, "options.xml"); } }

    [XmlElement("inverty")]
    public bool InvertYAxis;

    public static bool Exists()
    {
        return Exists(Filename);
    }

    public static OptionsFile ReadFromFile()
    {
        return ReadFromFile(Filename);
    }

    public static OptionsFile ReadOrNew()
    {
        if (Exists())
            return ReadFromFile();
        return new OptionsFile();
    }

    public void WriteToFile()
    {
        WriteToFile(Filename);
    }
}
