using UnityEngine;

public class Shield : MonoBehaviour
{
    public string Name;
    public int LootIndex;

    [Header("Base Upgradable Properties")]
    public float BaseCapacity;
    public float BaseRegenerationRate;

    public float Capacity { get { return BaseCapacity + CapacityPerPoint * CapacityPoints; } }
    public float RegenerationRate { get { return BaseRegenerationRate + RegenerationPerPoint * RegenerationRatePoints; } }

    [Header("Upgrade Status")]
    public int CapacityPoints;
    public int RegenerationRatePoints;

    [Header("Upgrading")]
    public float CapacityPerPoint = 5f;
    public float RegenerationPerPoint = 5f;

    public int CapacityPointCost { get { return 100 + 100 * CapacityPoints; } }
    public int RegenerationPointCost { get { return 100 + 100 * RegenerationRatePoints; } }
}
