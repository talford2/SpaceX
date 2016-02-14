using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	[Header("Parts")]
	public GameObject Head;
	public GameObject Guns;
	public List<GameObject> RecoilBarrels;

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
	public Targetable Targetable;

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

	private void Awake()
	{
		foreach (var shootPoint in ShootPoints)
		{
			shootPoint.Initialize();
		}

		_velocityReference = new VelocityReference(Vector3.zero);
		if (Targetable == null)
		{
			Targetable = GetComponent<Targetable>();
		}

		_weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_weaponInstance.Initialize(gameObject, ShootPoints, _velocityReference, Targetable.Team);

		for(var i=0; i < ShootPoints.Count; i++)
		{
			_recoilCooldowns.Add(0);
			//_weaponInstance.OnShoot += ... {
			//	_recoilCooldowns[i] = 0;
			//}
		}
	}

	private void Update()
	{
		var shootPointsCentre = _weaponInstance.GetShootPointCentre();

		for (var i = 0; i < _recoilCooldowns.Count; i++)
		{
			_recoilCooldowns[i] += Time.deltaTime;
			_recoilCooldowns[i] = Mathf.Min(_recoilCooldowns[i], RecoilTime);
			var frac = _recoilCooldowns[i] / RecoilTime;
			RecoilBarrels[i].transform.localPosition = Vector3.forward * RecoilCurve.Evaluate(frac);

		}

		if (_targetSearchCooldown >= 0f)
		{
			_targetSearchCooldown -= Time.deltaTime;
			if (_targetSearchCooldown < 0f)
			{
				_target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, Guns.transform.forward, MaxTargetDistance);
				if (_target == null)
					_target = Targeting.FindNearestTeam(Targeting.GetEnemyTeam(Targetable.Team), transform.position, MaxTargetDistance);
				_targetSearchCooldown = _targetSearchInterval;
				_aimCooldown = 0f;
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
						_aimPosition = Utility.GetVehicleExtrapolatedPosition(targetVehicle, _weaponInstance, 0f); //Random.Range(-ExtrapolationTimeError, ExtrapolationTimeError));

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
			var toTarget = _target.position - shootPointsCentre;
			var angleTo = Vector3.Angle(Guns.transform.forward, toTarget);
			var dontShoot = false;
			if (Mathf.Abs(angleTo) < AimTolerance)
			{
				_weaponInstance.SetAimAt(shootPointsCentre + Guns.transform.forward * MaxTargetDistance);
				RaycastHit aimHit;
				if (Physics.Raycast(new Ray(shootPointsCentre, Guns.transform.forward), out aimHit, MaxTargetDistance))
				{
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
				_weaponInstance.IsTriggered = !dontShoot;
			}
		}
		else
		{
			_weaponInstance.IsTriggered = false;
		}
	}
}