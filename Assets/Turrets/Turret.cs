using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Parts")]
    public GameObject Head;
    public GameObject Guns;
    public List<Transform> RecoilBarrels;

    public Weapon WeaponPrefab;
    public List<ShootPoint> ShootPoints;
    public float MaxTargetDistance;

    public float RecoilDistance = 0.2f;
    public float RecoilTime = 0.5f;
    public AnimationCurve RecoilCurve;

    [Header("Turning")]
    public float MinPitch;
    public float MaxPitch;
    public float MaxPitchSpeed = 90f;
    public float MaxYawSpeed = 90f;

    [Header("Aiming")]
    public float AimTolerance = 5f;
    public float AimTooCloseDistance = 10f;
    public Targetable Targetable;

    [Header("Targeting")]
    public float MinTargetSearchInterval = 2f;
    public float MaxTargetSearchInterval = 2f;

    [Header("Burst Firing")]
    public int BurstCount = 5;
    public float BurstWaitTime = 1.5f;

    private VelocityReference _velocityReference;
    private Weapon _weaponInstance;

    private Transform _target;
    private float _targetSearchCooldown;

    private float _aimIntervalMin = 0.1f;
    private float _aimIntervalMax = 0.7f;
    private float _aimCooldown;
    private Vector3 _aimPosition;

    private float _yaw;
    private float _pitch;

    private List<float> _recoilCooldowns;
    private List<Vector3> _barrelOffsets;

    private int _hitMask;

    // Burst Fire
    private int _fireCount;
    private float _burstCooldown;

    private void Awake()
    {
        foreach (var shootPoint in ShootPoints)
        {
            shootPoint.Initialize(WeaponPrefab.MuzzlePrefab);
        }

        _velocityReference = new VelocityReference(Vector3.zero);
        if (Targetable == null)
        {
            Targetable = GetComponent<Targetable>();
        }

        _weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
        _weaponInstance.Initialize(gameObject, ShootPoints, _velocityReference, Targetable.Team);
        _weaponInstance.OnShoot += OnShoot;

        _recoilCooldowns = new List<float>();
        _barrelOffsets = new List<Vector3>();
        for (var i = 0; i < ShootPoints.Count; i++)
        {
            _recoilCooldowns.Add(RecoilTime);
            _barrelOffsets.Add(RecoilBarrels[i].localPosition);
        }
        _weaponInstance.OnShoot += (shootPointIndex) =>
        {
            _recoilCooldowns[shootPointIndex] = RecoilTime;
        };

        _hitMask = ~LayerMask.GetMask("Distant", "Universe Background", "Detectable");

        var killable = GetComponent<Killable>();
        killable.OnDie += OnKilled;
    }

    private void Update()
    {
        var shootPointsCentre = _weaponInstance.GetShootPointCentre();
        var shootPointsForward = _weaponInstance.GetShootPointForward();

        for (var i = 0; i < _recoilCooldowns.Count; i++)
        {
            _recoilCooldowns[i] -= Time.deltaTime;
            _recoilCooldowns[i] = Mathf.Max(_recoilCooldowns[i], 0);
            var frac = 1 - _recoilCooldowns[i] / RecoilTime;
            RecoilBarrels[i].localPosition = _barrelOffsets[i] - Vector3.forward * RecoilCurve.Evaluate(frac) * RecoilDistance;
        }

        if (_targetSearchCooldown >= 0f)
        {
            _targetSearchCooldown -= Time.deltaTime;
            if (_targetSearchCooldown < 0f)
            {
                if (Targetable.Team != Team.Neutral)
                {
                    _target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, shootPointsForward, MaxTargetDistance);
                    if (_target == null)
                        _target = Targeting.FindNearestTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, MaxTargetDistance);
                }
                _targetSearchCooldown = Random.Range(MinTargetSearchInterval, MaxTargetSearchInterval);
                _aimCooldown = Random.Range(_aimIntervalMin, _aimIntervalMax);
            }
        }

        if (_target != null)
        {
            if (_aimCooldown >= 0f)
            {
                _aimCooldown -= Time.deltaTime;
                if (_aimCooldown < 0f)
                {
                    _aimPosition = _target.transform.position;
                    var targetVehicle = _target.GetComponent<Vehicle>();
                    if (targetVehicle != null)
                    {
                        _aimPosition = Utility.GetVehicleExtrapolatedPosition(targetVehicle, _weaponInstance, 0f);
                    }
                    _aimCooldown = Random.Range(_aimIntervalMin, _aimIntervalMax);
                }
            }
        }

        var toAimPosition = _aimPosition - shootPointsCentre;

        var targetYaw = GetLocalTargetEuler(Head.transform, _aimPosition).y;
        var targetPitch = GetLocalTargetEuler(Guns.transform, _aimPosition).x;

        Head.transform.localRotation = Quaternion.RotateTowards(Head.transform.localRotation, Quaternion.AngleAxis(targetYaw, Vector3.up), MaxYawSpeed * Time.deltaTime);
        Guns.transform.localRotation = Quaternion.RotateTowards(Guns.transform.localRotation, Quaternion.AngleAxis(targetPitch, Vector3.right), MaxPitchSpeed * Time.deltaTime);

        if (_burstCooldown >= 0f)
        {
            _burstCooldown -= Time.deltaTime;
            _weaponInstance.IsTriggered = false;
        }
        else
        {
            shootPointsForward = _weaponInstance.GetShootPointForward();
            var dontShoot = false;
            RaycastHit aimHit;
            if (Physics.Raycast(new Ray(shootPointsCentre, shootPointsForward), out aimHit, MaxTargetDistance, _hitMask))
            {
                if (aimHit.distance < AimTooCloseDistance)
                {
                    dontShoot = true;
                }
            }
            if (!dontShoot)
            {
                if (Mathf.Abs(Vector3.Angle(shootPointsForward, toAimPosition)) < AimTolerance)
                {
                    _weaponInstance.SetAimAt(shootPointsCentre + toAimPosition.normalized * MaxTargetDistance);
                }
                else
                {
                    dontShoot = true;
                }
            }
            if (_target != null)
            {
                _weaponInstance.SetMissileTarget(_target);
            }
            else
            {
                dontShoot = true;
            }
            _weaponInstance.IsTriggered = !dontShoot;
        }
    }

    private Vector3 GetLocalTargetEuler(Transform trans, Vector3 lookAt)
    {
        return Quaternion.LookRotation(trans.InverseTransformPoint(lookAt), trans.up).eulerAngles + trans.localEulerAngles;
    }

    private void OnShoot(int shootPointIndex)
    {
        _fireCount++;
        if (_fireCount >= BurstCount)
        {
            _burstCooldown = BurstWaitTime;
            _weaponInstance.IsTriggered = false;
            _fireCount = 0;
        }
    }

    private void OnKilled(Killable sender, GameObject attacker)
    {
        if (attacker != null)
        {
            var attackerTargetable = attacker.GetComponentInChildren<Targetable>();
            if (attackerTargetable != null)
                HeadsUpDisplay.Current.RecordKill(attackerTargetable.Team);
        }
    }
}