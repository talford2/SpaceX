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
	public float ExtrapolationTimeError = 0.5f;
    //public float AimOffsetRadius = 5f;
    public float AimDistanceOffsetMultiplier = 0f;
    public float AimTooCloseDistance = 10f;
    public Targetable Targetable;

    [Header("Burst Firing")]
    public int BurstCount = 5;
    public float BurstWaitTime = 1.5f;

	private VelocityReference _velocityReference;
	private Weapon _weaponInstance;

	private Transform _target;
	private float _targetSearchInterval = 2f;
	private float _targetSearchCooldown;

	private float _aimInterval = 0.3f;
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
                    _target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, Guns.transform.forward, MaxTargetDistance);
                    if (_target == null)
                        _target = Targeting.FindNearestTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, MaxTargetDistance);
                }
				_targetSearchCooldown = _targetSearchInterval;
				_aimCooldown = Random.Range(0, 0.5f);
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
                        var targetPosition = Utility.GetVehicleExtrapolatedPosition(targetVehicle, _weaponInstance, 0f);
                        var distance = (targetPosition - transform.position).magnitude;
                        var offset = Random.insideUnitSphere * distance * AimDistanceOffsetMultiplier;
                        _aimPosition = targetPosition + offset;
                    }

					_aimCooldown = _aimInterval;
				}
			}

			Head.transform.LookAt(_aimPosition, Vector3.up);
			var targetYaw = Head.transform.localEulerAngles.y;
			//var targetYaw = Quaternion.LookRotation(aimPosition - Head.transform.position, Vector3.up).eulerAngles.y; // this is world space not local!

			_yaw = Mathf.MoveTowardsAngle(_yaw, targetYaw, MaxYawSpeed * Time.deltaTime);
			Head.transform.localRotation = Quaternion.Euler(0, _yaw, 0);

			Guns.transform.LookAt(_aimPosition, Vector3.forward);
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
			_pitch = Mathf.MoveTowardsAngle(_pitch, targetPitch, MaxPitchSpeed * Time.deltaTime);
			Guns.transform.localRotation = Quaternion.Euler(targetPitch, 0, 0);

            // Shooting
            if (_burstCooldown > 0f)
            {
                _burstCooldown -= Time.deltaTime;
            }
            else
            {
                var toAim = _aimPosition - shootPointsCentre;
                var angleTo = Vector3.Angle(Guns.transform.forward, toAim);
                var dontShoot = false;
                if (Mathf.Abs(angleTo) < AimTolerance)
                {
                    _weaponInstance.SetAimAt(shootPointsCentre + Guns.transform.forward * MaxTargetDistance);
                    RaycastHit aimHit;
                    if (Physics.Raycast(new Ray(shootPointsCentre, Guns.transform.forward), out aimHit, MaxTargetDistance, _hitMask))
                    {
                        if (aimHit.distance < AimTooCloseDistance)
                        {
                            dontShoot = true;
                        }
                        var aimAtTargetable = aimHit.collider.GetComponentInParent<Targetable>();
                        if (aimAtTargetable != null)
                        {
                            if (Targetable.Team == aimAtTargetable.Team)
                            {
                                dontShoot = true;
                            }
                        }
                        else
                        {
                            var mothership = aimHit.collider.GetComponentInParent<Mothership>();
                            if (mothership != null)
                            {
                                dontShoot = true;
                            }
                        }
                    }
                    _weaponInstance.SetMissileTarget(_target);
                    _weaponInstance.IsTriggered = !dontShoot;
                }
            }
		}
		else
		{
			_weaponInstance.IsTriggered = false;
		}
	}

    private void OnShoot(int shootPointIndex)
    {
        _fireCount++;
        if (_fireCount > BurstCount)
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
            var attackerTargetable = attacker.GetComponent<Targetable>();
            if (attackerTargetable != null)
                HeadsUpDisplay.Current.RecordKill(attackerTargetable.Team);
        }
    }
}