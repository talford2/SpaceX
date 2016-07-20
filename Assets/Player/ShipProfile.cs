using UnityEngine;

public class ShipProfile : MonoBehaviour
{
    public string CallSign;
    public int TotalPower = 15;
    public int Weapons = 5;
    public int Shields = 5;
    public int Special = 1;

    public Weapon PrimaryWeapon;
    public Weapon SecondaryWeapon;
    public Shield Shield;

    public int PowerRemaining { get { return TotalPower - Weapons - Shields - Special; } }

    public float GetShield()
    {
        return 15f * Shields;
    }

    public float GetBoostEnergy()
    {
        return 15f * Special;
    }

    public string ToJson()
    {
        var obj = new JsonPowerProfile
        {
            callsign = CallSign,
            power = TotalPower,
            weapons = Weapons,
            shields = Shields,
            special = Special
        };
        
        return JsonUtility.ToJson(obj);
    }

    public class JsonPowerProfile
    {
        public string callsign;
        public int power;
        public int weapons;
        public int shields;
        public int special;
    }
}
