using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	[Header("Vehicle Settings")]
	public float IdleSpeed = 10f;

	public float MaxSpeed = 20f;

	public float MinSpeed = 3f;

	public float Acceleration = 5f;

	public float Brake = 7f;

	public float MaxBoostSpeed = 500f;

	public float BoostAcceleration = 500f;

	public float BoostBrake = 150f;

	public float CurrentSpeed = 0f;

	public float PitchSpeed = 5f;

	public float YawSpeed = 5f;

	public float RollAcceleration = 640f;

	public float MaxRollSpeed = 100f;

    [Header("Collisions")]
    public Vector3 CollisionsCentre;
    public float CollisionRadius = 3f;

	[Header("Boost Energy")]
	public float MaxBoostEnergy = 100f;

	public float BoostEnergy = 100f;

	public float BoostCost = 50f;

	public float BoostEnergyRegenerateDelay = 5f;

	public float BoostEnergyRegenerateRate = 1f;

	[Header("Control Settings")]
	public bool TriggerAccelerate = false;

	public bool TriggerBrake = false;

	public bool TriggerBoost = false;

	public float YawThrottle = 0f;

	public float PitchThotttle = 0f;

	public float RollThrottle = 0f;

	public bool IsAccelerating { get { return TriggerAccelerate; } }

	public bool IsBraking { get { return TriggerBrake; } }

	public bool IsBoosting { get { return TriggerBoost && BoostEnergy > 0f && allowBoost; } }

	[Header("Primary Weapon")]
	public Weapon PrimaryWeaponPrefab;

	public List<ShootPoint> PrimaryShootPoints;

	[Header("Secondary Weapon")]
	public Weapon SecondaryWeaponPrefab;

	public List<ShootPoint> SecondaryShootPoints;

	[Header("Other")]
	public List<Thruster> Thrusters;

	public AudioSource BoostSound;

	private Shiftable _shiftable;

	private Weapon _primaryWeaponInstance;
	private Weapon _secondaryWeaponInstance;

	private Vector3 _velocity;
	private VelocityReference _velocityReference;

	private readonly float _aimDistance = 1000f;

	// Steering
	private float rollSpeed;
	private Quaternion targetRotation;

	// Boost
	private float boostEnergyCooldown;
	private bool boostRegenerate;
	private bool allowBoost;

	private float maxFlareBrightness = 30f;
	public Weapon PrimaryWeaponInstance { get { return _primaryWeaponInstance; } }
	public Weapon SecondaryWeaponInstance { get { return _secondaryWeaponInstance; } }

	public Shiftable Shiftable
	{
		get
		{
			if (_shiftable == null)
			{
				_shiftable = GetComponent<Shiftable>();
			}
			return _shiftable;
		}
	}

    public Vector3 GetAimPosition()
    {
        return _primaryWeaponInstance.GetShootPointCentre() + _aimDistance*transform.forward;
    }

    private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();

		foreach (var shootPoint in PrimaryShootPoints)
		{
			shootPoint.Initialize();
		}
		foreach (var shootPoint in SecondaryShootPoints)
		{
			shootPoint.Initialize();
		}
		_velocityReference = new VelocityReference(_velocity);

		var targetable = GetComponent<Targetable>();

		_primaryWeaponInstance = Utility.InstantiateInParent(PrimaryWeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_primaryWeaponInstance.Initialize(gameObject, PrimaryShootPoints, _velocityReference, targetable.Team);
		_primaryWeaponInstance.OnShoot += OnShoot;

		_secondaryWeaponInstance = Utility.InstantiateInParent(SecondaryWeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_secondaryWeaponInstance.Initialize(gameObject, SecondaryShootPoints, _velocityReference, targetable.Team);
		_secondaryWeaponInstance.OnShoot += OnShoot;

		allowBoost = true;

		targetRotation = transform.rotation;
	}

	public void SetAimAt(Vector3 aimAt)
	{
		_primaryWeaponInstance.SetAimAt(aimAt);
		_secondaryWeaponInstance.SetAimAt(aimAt);
	}

	private void OnShoot()
	{
	}

	private void Update()
	{
		var acceleration = 0f;

		if (!TriggerBoost)
		{
			// Accelerating
			if (TriggerAccelerate && CurrentSpeed < MaxSpeed)
			{
				acceleration = Acceleration;
				CurrentSpeed += Acceleration * Time.deltaTime;
				CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
			}

			// Braking
			if (TriggerBrake && CurrentSpeed > MinSpeed)
			{
				acceleration = -Brake;
				CurrentSpeed -= Brake * Time.deltaTime;
				CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
			}

			allowBoost = true;
		}

		// Restore boost energy
		if (boostEnergyCooldown > 0f)
		{
			boostEnergyCooldown -= Time.deltaTime;
			if (boostEnergyCooldown < 0f)
			{
				boostRegenerate = true;
			}
		}

		if (boostRegenerate)
		{
			if (BoostEnergy < MaxBoostEnergy)
			{
				BoostEnergy += BoostEnergyRegenerateRate * Time.deltaTime;
				if (BoostEnergy > MaxBoostEnergy)
					BoostEnergy = MaxBoostEnergy;
			}
		}

		// Boosting
		if (TriggerBoost)
		{
			if (allowBoost && BoostEnergy > 0f)
			{
				if (CurrentSpeed < MaxBoostSpeed)
				{
					acceleration = BoostAcceleration;
					CurrentSpeed += BoostAcceleration * Time.deltaTime;
					CurrentSpeed = Mathf.Min(CurrentSpeed, MaxBoostSpeed);
				}
				BoostEnergy -= BoostCost * Time.deltaTime;
				if (BoostEnergy < 0f)
				{
					BoostEnergy = 0f;
					allowBoost = false;
				}
				boostRegenerate = false;
				boostEnergyCooldown = BoostEnergyRegenerateDelay;
			}
			if (!BoostSound.isPlaying)
			{
				BoostSound.Play();
			}
		}

		if (!IsBoosting)
		{
			if (CurrentSpeed > MaxSpeed)
			{
				CurrentSpeed -= BoostBrake * Time.deltaTime;
			}
			BoostSound.Stop();
		}

		// Idling
		if (!IsAccelerating && !IsBraking && !IsBoosting)
		{
			if (CurrentSpeed > IdleSpeed)
			{
				acceleration = -Brake;
				CurrentSpeed -= Brake * Time.deltaTime;
				CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
			}

			if (CurrentSpeed < IdleSpeed)
			{
				acceleration = Acceleration;
				CurrentSpeed += Acceleration * Time.deltaTime;
				CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
			}
		}

		rollSpeed += RollAcceleration * Mathf.Clamp(RollThrottle, -1, 1) * Time.deltaTime;

		if (Mathf.Abs(RollThrottle) < 0.01f)
		{
			rollSpeed = Mathf.Lerp(rollSpeed, 0f, 10f * Time.deltaTime);
		}

		// Turning
		var dYaw = YawThrottle * YawSpeed;
		var dPitch = PitchThotttle * PitchSpeed;
		var dRoll = -Mathf.Clamp(rollSpeed, -MaxRollSpeed, MaxRollSpeed) * Time.deltaTime;

		targetRotation *= Quaternion.Euler(dPitch, dYaw, dRoll);

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20f * Time.deltaTime);

		_velocity = transform.forward * CurrentSpeed;

        UpdateVelocityFromCollisions();

        _velocityReference.Value = _velocity;
		_shiftable.Translate(_velocity * Time.deltaTime);

		var thrustAmount = acceleration / MaxSpeed;

		if (IsBoosting)
		{
			thrustAmount = 1f;
		}
		else
		{
			if (IsAccelerating)
			{
				thrustAmount = 0.08f;
			}
			if (IsBraking)
			{
				thrustAmount = 0f;
			}
		}
		if (!IsBoosting && !IsAccelerating && !IsBraking)
		{
			thrustAmount = 0.02f;
		}

		// Reduce flare brightness over distance from camera
		foreach (var thruster in Thrusters)
		{
			thruster.SetAmount(thrustAmount);
			thruster.UpdateFlare();
		}
	}

    private void UpdateVelocityFromCollisions()
    {
        /*
        var moveRay = new Ray(transform.position + CollisionsCentre, _velocity);
        var moveHits = Physics.SphereCastAll(moveRay, CollisionRadius, _velocity.magnitude * Time.deltaTime, LayerMask.GetMask("Environment"));
        foreach (var moveHit in moveHits)
        {
            var projVel = Vector3.Project(_velocity, moveHit.normal);
            _velocity -= projVel;
        }
        */
    }

    public Vector3 GetVelocity()
	{
		return _velocity;
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + CollisionsCentre, CollisionRadius);
    }
}
