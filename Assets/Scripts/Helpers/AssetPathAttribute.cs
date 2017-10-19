using UnityEngine;

public class AssetPathAttribute : PropertyAttribute
{
    public readonly string Path;

    public AssetPathAttribute(string path)
    {
        Path = path;
    }
}
