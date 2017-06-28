using UnityEngine;

[CreateAssetMenu(fileName = "BluePrint", menuName = "SpaceX/Blue Print")]
public class BluePrint : ScriptableObject
{
    public string Key { get { return name; } }
    public int RequiredCount;
    public Object Item;
    public int Price;
}
