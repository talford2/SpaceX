using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Description")]
    public string Name;
    public int LootIndex;

    [Header("Weapon Settings")]
    public GameObject MissilePrefab;

    public MuzzleFlash MuzzlePrefab;

    //public AudioSource FireSound;
    [Header("Shoot Sound")]
    public GameObject SoundPrefab;
    public AudioClip ShootSound;
    public float SoundVolume = 1f;

    public int MissilesPerShot = 2;
    public float Spread = 0f;
    public bool MissilesConverge;

    // Targeting
    public bool IsTargetLocking;
    public float TargetLockTime = 1.5f;
    public float TargetLockingMaxDistance = 2000f;

    // Overheating
    public bool IsOverheat;
    public float HeatPerMissile;
    public float OverheatDelay;
    public float CoolDelay = 0.5f;

    public AudioClip LockSound;

    [Header("Base Upgradable Properties")]
    public float BaseMissileDamage;
    public float BaseFireRate = 0.2f;
    public float BaseCoolingRate;
    public float BaseOverheatValue;

    public float Damage { get { return BaseMissileDamage + DamagePerPoint * DamagePoints; } }
    public float FireRate { get { return BaseFireRate + FireRatePerPoint * FireRatePoints; } }
    public float CoolingRate { get { return BaseCoolingRate + CoolingRatePerPoint * CoolingRatePoints; } }
    public float OverheatValue { get { return BaseOverheatValue + HeatCapacityPerPoint * HeatCapacityPoints; } }

    [Header("Upgrade Status")]
    public int DamagePoints;
    public int FireRatePoints;
    public int CoolingRatePoints;
    public int HeatCapacityPoints;

    [Header("Upgrading")]
    public float DamagePerPoint;
    public float FireRatePerPoint;
    public float CoolingRatePerPoint;
    public float HeatCapacityPerPoint;

    public int DamagePointCost { get { return 100 + 100 * DamagePoints; } }
    public int FireRatePointCost { get { return 100 + 100 * FireRatePoints; } }
    public int CoolingRatePointCost { get { return 100 + 100 * CoolingRatePoints; } }
    public int HeatCapacityPointCost { get { return 100 + 100 * HeatCapacityPoints; } }

    private float _fireCooldown = 0f;
    private List<ShootPoint> _shootPoints;
    private int _shootPointIndex;
    private Vector3 _aimAt;

    private VelocityReference _velocityReference;

    public bool IsTriggered;

    public delegate void OnShootEvent(int shootPointIndex);
    public OnShootEvent OnShoot;

    private GameObject _owner;
    private Team _ownerTeam;

    private Team _targetTeam;

    private Transform _lockingTarget;
    private Transform _lastLockingTarget;
    private Transform _lockedTarget;
    private float _lockingCooldown;
    private bool _isLocked;

    private Transform _missileTarget; // Missile target for non-locking missiles.

    private float heatValue;
    private float heatCooldown;
    private bool isCoolingDown;
    private float coolDelayCooldown;

    public void Initialize(GameObject owner, List<ShootPoint> shootPoints, VelocityReference velocityReference, Team ownerTeam)
    {
        _owner = owner;
        _shootPoints = shootPoints;
        _shootPointIndex = 0;
        _velocityReference = velocityReference;
        _ownerTeam = ownerTeam;
        _targetTeam = Targeting.GetEnemyTeam(_ownerTeam);
        foreach (var shootPoint in shootPoints)
        {
            shootPoint.Initialize(MuzzlePrefab);
        }
        heatValue = 0f;
        isCoolingDown = false;
        heatCooldown = 0f;
    }

    private void Update()
    {
        //Debug.LogFormat("{0} ===", IsTriggered);

        var isOverheated = false;
        if (heatCooldown >= 0f)
        {
            isOverheated = true;
            heatCooldown -= Time.deltaTime;
            if (heatCooldown < 0f)
            {
                isCoolingDown = true;
                isOverheated = false;
            }
        }
        else
        {
            if (coolDelayCooldown >= 0f)
            {
                coolDelayCooldown -= Time.deltaTime;
                if (coolDelayCooldown < 0f)
                {
                    isCoolingDown = true;
                }
            }
        }

        if (isCoolingDown)
        {
            heatValue -= CoolingRate * Time.deltaTime;
            if (heatValue < 0f)
            {
                heatValue = 0f;
                isOverheated = false;
                isCoolingDown = false;
            }
        }

        if ((IsOverheat && !isOverheated) || !IsOverheat)
        {
            if (!IsTargetLocking)
            {
                _fireCooldown -= Time.deltaTime;
                if (IsTriggered && _fireCooldown < 0)
                {
                    //Debug.Log("Shoot!");
                    _fireCooldown = 1f / FireRate;
                    Fire();
                }
            }
            else
            {
                TargetLocking();
            }
        }
    }

    public Vector3 GetShootPointCentre()
    {
        return _shootPoints.Aggregate(Vector3.zero, (current, shootPoint) => current + shootPoint.transform.position) / _shootPoints.Count;
    }

    public void SetAimAt(Vector3 aimAt)
    {
        _aimAt = aimAt;
    }

    private GameObject _missileInstance;
    private GameObject GetNextMissile()
    {
        _missileInstance = ResourcePoolManager.GetAvailable(MissilePrefab, Vector3.zero, Quaternion.identity);
        _missileInstance.GetComponent<Missile>().Initialize(_owner, Damage);
        return _missileInstance;
    }

    private ShootPoint _shootPoint;
    public void FireMissile(GameObject missile)
    {
        if (OnShoot != null)
            OnShoot(_shootPointIndex);

        _shootPoint = _shootPoints[_shootPointIndex];

        var direction = _aimAt - _shootPoint.transform.position;
        if (!MissilesConverge)
            direction += _shootPoint.transform.position - GetShootPointCentre();
        missile.GetComponent<Missile>().FromReference = _shootPoint.transform;
        missile.GetComponent<Missile>().Shoot(_shootPoint.transform.position, Quaternion.Euler(Random.Range(-0.5f * Spread, 0.5f * Spread), Random.Range(-0.5f * Spread, 0.5f * Spread), 0f) * direction, _velocityReference.Value);

        _shootPoint.Flash();

        var fireSound = ResourcePoolManager.GetAvailable(SoundPrefab, _shootPoint.transform.position, Quaternion.identity).GetComponent<AnonymousSound>();
        fireSound.PlayAt(ShootSound, _shootPoint.transform.position, SoundVolume);
        //FireSound.Play();

        if (IsOverheat)
        {
            heatValue += HeatPerMissile;
            isCoolingDown = false;
            if (heatValue >= OverheatValue)
            {
                heatCooldown = OverheatDelay;
            }
            else
            {
                coolDelayCooldown = CoolDelay;
            }
        }

        _shootPointIndex++;
        if (_shootPointIndex >= _shootPoints.Count)
            _shootPointIndex = 0;
    }

    public void Fire()
    {
        for (var i = 0; i < MissilesPerShot; i++)
        {
            var nextMissile = GetNextMissile();
            if (_missileTarget != null)
                nextMissile.GetComponent<Missile>().SetTarget(_missileTarget);
            FireMissile(nextMissile);
        }
    }

    public void ClearTargetLock()
    {
        _isLocked = false;
        _lastLockingTarget = null;
        _lockedTarget = null;
    }

    private Vehicle _lockedVehicle;
    private GameObject _nextMissile;
    private Targetable _targetable;

    private void TargetLocking()
    {
        var shootPointsCentre = GetShootPointCentre();
        if (_lockingTarget != null)
        {
            var toLockingTarget = _lockingTarget.position - shootPointsCentre;
            if (toLockingTarget.sqrMagnitude > TargetLockingMaxDistance * TargetLockingMaxDistance)
            {
                ClearTargetLock();
            }
        }
        else
        {
            ClearTargetLock();
        }
        if (_lockedTarget != null)
        {
            var toLockedTarget = _lockedTarget.position - shootPointsCentre;
            if (toLockedTarget.sqrMagnitude > TargetLockingMaxDistance * TargetLockingMaxDistance)
            {
                ClearTargetLock();
            }
        }
        else
        {
            ClearTargetLock();
        }

        if (!_isLocked)
        {
            _lockingTarget = null;
            if (IsTriggered)
            {
                var targetLockingDir = _aimAt - shootPointsCentre;
                _lockingTarget = Targeting.FindFacingAngleTeam(_targetTeam, shootPointsCentre, targetLockingDir, TargetLockingMaxDistance);
                if (_lastLockingTarget == null)
                    _lastLockingTarget = _lockingTarget;
            }
            else
            {
                _lastLockingTarget = null;
            }

            if (_lastLockingTarget != null && _lastLockingTarget == _lockingTarget)
            {
                _lockingCooldown -= Time.deltaTime;
                if (_lockingCooldown < 0f)
                {
                    _lockedTarget = _lockingTarget;
                    _isLocked = true;
                    if (LockSound != null)
                    {
                        Utility.PlayOnTransform(LockSound, _owner.transform);
                    }
                }
            }
            else
            {
                _lockingCooldown = TargetLockTime;
            }
        }
        else
        {
            if (!IsTriggered)
            {
                _lockedVehicle = _lockedTarget.GetComponent<Vehicle>();
                if (_lockedVehicle != null)
                {
                    // Rough Extrapolation
                    SetAimAt(Utility.GetVehicleExtrapolatedPosition(_lockedVehicle, this, 0f));
                }
                else
                {
                    SetAimAt(_lockedTarget.position);
                }

                _targetable = _lockedTarget.GetComponent<Targetable>();
                if (_targetable != null)
                {
                    _targetable.LockedOnBy = _owner.transform;
                }

                SetAimAt(GetShootPointCentre() + _velocityReference.Value);
                _nextMissile = GetNextMissile();
                _nextMissile.GetComponent<Missile>().SetTarget(_lockedTarget);
                FireMissile(_nextMissile);
                ClearTargetLock();
            }
        }
    }

    public Transform GetLockingOnTarget()
    {
        return _lockingTarget;
    }

    public Transform GetLockedOnTarget()
    {
        return _lockedTarget;
    }

    public float GetHeatFraction()
    {
        return Mathf.Clamp01(heatValue / OverheatValue);
    }

    public void SetMissileTarget(Transform target)
    {
        _missileTarget = target;
    }

    private void OnDestroy()
    {
        ClearTargetLock();
    }
}
