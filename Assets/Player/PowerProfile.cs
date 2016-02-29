using UnityEngine;

public class PowerProfile : MonoBehaviour
{
    public int TotalPower = 15;
    public int Weapons = 5;
    public int Shields = 5;
    public int Special = 1;

    public int PowerRemaining { get { return TotalPower - Weapons - Shields - Special; } }
}
