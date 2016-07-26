using UnityEngine;

public class Engine : MonoBehaviour
{
    public string Name;
    public int LootIndex;

    [Header("Base Upgradable Properties")]
    public float BaseAcceleration;
    public float BaseBoostEnergy;

    public float Acceleration { get { return BaseAcceleration + AccelerationPerPoint * AccelerationPoints; } }
    public float BoostEnergy {  get { return BaseBoostEnergy + BoostEnergyPerPoint * BoostEnergyPoints; } }

    [Header("Upgrade Status")]
    public int AccelerationPoints;
    public int BoostEnergyPoints;

    [Header("Upgrading")]
    public float AccelerationPerPoint = 2.5f;
    public float BoostEnergyPerPoint = 5f;

    public int AccelerationPointCost { get { return 100 + 100 * AccelerationPoints; } }
    public int BoostEnergyPointCost { get { return 100 + 100 * BoostEnergyPoints; } }

}
