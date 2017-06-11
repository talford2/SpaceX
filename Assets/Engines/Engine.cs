using UnityEngine;

public class Engine : MonoBehaviour
{
    public int LootIndex;
    public EngineDefinition Definition;

    public float Acceleration { get { return Definition.BaseAcceleration + Definition.AccelerationPerPoint * AccelerationPoints; } }
    public float BoostEnergy {  get { return Definition.BaseBoostEnergy + Definition.BoostEnergyPerPoint * BoostEnergyPoints; } }

    [Header("Upgrade Status")]
    public int AccelerationPoints;
    public int BoostEnergyPoints;

    public int AccelerationPointCost { get { return 100 + 100 * AccelerationPoints; } }
    public int BoostEnergyPointCost { get { return 100 + 100 * BoostEnergyPoints; } }

}
