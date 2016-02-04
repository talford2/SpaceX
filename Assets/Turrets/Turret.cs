using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Parts")]
    public GameObject Head;
    public GameObject Guns;

    public Weapon WeaponPrefab;
    public List<ShootPoint> ShootPoints;
    public float MaxTargetDistance;

    [Header("Turning")]
    public float MinPitch;
    public float MaxPitch;
    public float MaxPitchSpeed = 90f;
    public float MaxYawSpeed = 90f;

    [Header("Aiming")]
    public float AimTolerance = 5f;
    public float ExtrapolationTimeError = 0.5f;

    private Targetable _targetable;
    private VelocityReference _velocityReference;
    private Weapon _weaponInstance;

    private Transform _target;
    private float targetSearchInterval = 2f;
    private float targetSearchCooldown;


    private float aimInterval = 0.3f;
    private float aimCooldown;
    private Vector3 aimPosition;

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        foreach (var shootPoint in ShootPoints)
        {
            shootPoint.Initialize();
        }

        _velocityReference = new VelocityReference(Vector3.zero);
        _targetable = GetComponent<Targetable>();

        _weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
        _weaponInstance.Initialize(gameObject, ShootPoints, _velocityReference, _targetable.Team);
    }

    private void Update()
    {
        var shootPointsCentre = _weaponInstance.GetShootPointCentre();

        if (targetSearchCooldown >= 0f)
        {
            targetSearchCooldown -= Time.deltaTime;
            if (targetSearchCooldown < 0f)
            {
                _target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(_targetable.Team), transform.position, Guns.transform.forward, MaxTargetDistance);
                if (_target == null)
                    _target = Targeting.FindNearestTeam(Targeting.GetEnemyTeam(_targetable.Team), transform.position, MaxTargetDistance);
                targetSearchCooldown = targetSearchInterval;
                aimCooldown = 0f;
            }
        }

        if (_target != null)
        {
            if (aimCooldown >= 0f)
            {
                aimCooldown -= Time.deltaTime;
                if (aimCooldown < 0f)
                {
                    aimPosition = _target.transform.position;

                    var targetVehicle = _target.GetComponent<Vehicle>();
                    if (targetVehicle != null)
                        aimPosition = Utility.GetVehicleExtrapolatedPosition(targetVehicle, _weaponInstance, 0f); //Random.Range(-ExtrapolationTimeError, ExtrapolationTimeError));

                    aimCooldown = aimInterval;
                }
            }

            Head.transform.LookAt(aimPosition, Vector3.up);
            var targetYaw = Head.transform.localEulerAngles.y;
            //var targetYaw = Quaternion.LookRotation(aimPosition - Head.transform.position, Vector3.up).eulerAngles.y; // this is world space not local!

            _yaw = Mathf.MoveTowardsAngle(_yaw, targetYaw, MaxYawSpeed*Time.deltaTime);
            Head.transform.localRotation = Quaternion.Euler(0, _yaw, 0);

            Guns.transform.LookAt(aimPosition, Vector3.forward);
            var targetPitch = Guns.transform.localEulerAngles.x;
            //var targetPitch = Quaternion.LookRotation(aimPosition - Guns.transform.position, Vector3.forward).eulerAngles.x; // this is world space not local!

            var deltaMin = Mathf.DeltaAngle(targetPitch, MinPitch);
            var deltaMax = Mathf.DeltaAngle(targetPitch, MaxPitch);

            if (deltaMin < 0f)
            {
                // Lower Limit
                targetPitch = MinPitch;
            }
            else
            {
                if (deltaMax > 0f)
                {
                    // Upper Limit
                    targetPitch = MaxPitch;
                }
            }
            _pitch = Mathf.MoveTowardsAngle(_pitch, targetPitch, MaxPitchSpeed*Time.deltaTime);
            Guns.transform.localRotation = Quaternion.Euler(targetPitch, 0, 0);

            // Shooting
            var toTarget = _target.position - shootPointsCentre;
            var angleTo = Vector3.Angle(Guns.transform.forward, toTarget);
            if (Mathf.Abs(angleTo) < AimTolerance)
            {
                _weaponInstance.SetAimAt(shootPointsCentre + Guns.transform.forward * MaxTargetDistance);
                RaycastHit aimHit;
                if (Physics.Raycast(new Ray(shootPointsCentre, Guns.transform.forward), out aimHit, MaxTargetDistance))
                {
                    var aimAtTargetable = aimHit.collider.GetComponentInParent<Targetable>();
                    if (aimAtTargetable != null)
                    {
                        if (_targetable.Team == aimAtTargetable.Team)
                        {
                            _weaponInstance.IsTriggered = false;
                        }
                    }
                }
                _weaponInstance.IsTriggered = true;
            }
        }
        else
        {
            _weaponInstance.IsTriggered = false;
        }
    }

}