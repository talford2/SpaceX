using UnityEngine;

[CreateAssetMenu(fileName = "Engine", menuName = "SpaceX/Engine", order = 1)]
public class EngineDefinition : ScriptableObject
{
    public string Name;

    [Header("Base Upgradable Properties")]
    public float BaseAcceleration;
    public float BaseBoostEnergy;

    [Header("Upgrading")]
    public float AccelerationPerPoint = 2.5f;
    public float BoostEnergyPerPoint = 5f;

    [Header("Inventory")]
    public Sprite InventorySprite;
    public ItemType Type;
    public int SalvageValue;
}
