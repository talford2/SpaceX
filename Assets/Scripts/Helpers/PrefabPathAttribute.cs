using UnityEngine;

public class PrefabPathAttribute : PropertyAttribute
{
    public readonly string Path;

    public PrefabPathAttribute(string path)
    {
        Path = path;
    }
}
