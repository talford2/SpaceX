using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "SpaceX/Weapon", order = 2)]
public class WeaponDefinition : ScriptableObject
{
    public string Name;
    public string Key { get { return name; } }

    [Header("Muzzle")]
    public MuzzleFlash MuzzlePrefab;

    [Header("Shoot Sound")]
    public AudioClip ShootSound;
    public float SoundVolume = 1f;
    public float SoundMinDistance = 25;
    public float SoundMaxDistance = 200f;

    [Header("Missile")]
    public GameObject MissilePrefab;
    public int MissilesPerShot = 1;
    public float Spread = 0f;
    public bool MissilesConverge;

    [Header("Targeting")]
    public bool IsTargetLocking;
    public bool IsAutoLocking;
    public float TargetLockTime = 1f;
    public float TargetLockingMaxDistance = 2000f;
    public float TargetAngleTolerance = 15f;
    public AudioClip LockSound;

    [Header("Burst Fire")]
    public bool IsBurstFire;
    public float BurstInterval;
    public int BurstShotCount;

    [Header("Overheating")]
    public bool IsOverheat;
    public float HeatPerMissile;
    public float OverheatDelay;
    public float CoolDelay;

    [Header("Base Upgradable Properties")]
    public float BaseMissileDamage;
    public float BaseFireRate;
    public float BaseCoolingRate;
    public float BaseHeatCapacity;

    [Header("Upgrading")]
    public float MissileDamagePerPoint;
    public float FireRatePerPoint;
    public float CoolingRatePerPoint;
    public float HeatCapacityPerPoint;

    [Header("Inventory")]
    public Sprite InventorySprite;
    public ItemType Type;
    public int SalvageValue;
}