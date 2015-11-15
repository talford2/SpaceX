using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	[Header("Vehicle Settings")]
	public float IdleSpeed = 10f;

	public float MaxSpeed = 20f;

	public float MinSpeed = 3f;

	public float Acceleration = 5f;

	public float Brake = 7f;

    public float BoostAcceleration = 500f;

	public float CurrentSpeed = 0f;

	public float PitchSpeed = 5f;

	public float YawSpeed = 5f;

    public float RollAcceleration = 640f;

    public float MaxRollSpeed = 100f;

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

    public bool IsAccelerating { get {  return TriggerAccelerate;} }

    public bool IsBraking { get { return TriggerBrake;} }

    public bool IsBoosting { get { return TriggerBoost && BoostEnergy > 0f && allowBoost; } }

	[Header("Weapon")]
	public Weapon WeaponPrefab;

	public List<ShootPoint> ShootPoints;

    [Header("Other")]
    public List<Thruster> Thrusters;

	private Shiftable _shiftable;

	private int _shootPointIndex;
    private Vector3 _aimAt;

	private Weapon _weaponInstance;

    private Vector3 _velocity;

    private readonly float _aimDistance = 1000f;
    private readonly float _yawClamp = 20f;
    private readonly float _pitchClamp = 20f;

    private float boostEnergyCooldown;
    private bool boostRegenerate;
    private bool allowBoost;

    private float maxFlareBrightness = 30f;
	public Weapon CurrentWeapon { get { return _weaponInstance; } }

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

    public Vector3 GetShootPointCentre()
    {
        return ShootPoints.Aggregate(Vector3.zero, (current, shootPoint) => current + shootPoint.transform.position)/ShootPoints.Count;
    }

    public Vector3 GetAimPosition()
    {
        return GetShootPointCentre() + _aimDistance*transform.forward;
    }

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shootPointIndex = 0;

	    foreach (var shootPoint in ShootPoints)
	    {
	        shootPoint.Initialize();
	    }

		_weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_weaponInstance.Initialize(gameObject);
		_weaponInstance.OnShoot += OnShoot;

	    allowBoost = true;
	}

    public void SetAimAt(Vector3 aimAt)
    {
        _aimAt = aimAt;
    }

    private void OnShoot()
    {
        ShootPoints[_shootPointIndex].transform.forward = _aimAt - ShootPoints[_shootPointIndex].transform.position;
        _weaponInstance.SetShootPoint(ShootPoints[_shootPointIndex], _velocity);
        _shootPointIndex++;
        if (_shootPointIndex >= ShootPoints.Count)
            _shootPointIndex = 0;
    }

    private float rollSpeed;
    private void Update()
    {
        var acceleration = 0f;

        if (!TriggerBoost)
        {
            // Accelerating
            if (TriggerAccelerate && CurrentSpeed < MaxSpeed)
            {
                acceleration = Acceleration;
                CurrentSpeed += Acceleration*Time.deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
            }

            // Braking
            if (TriggerBrake && CurrentSpeed > MinSpeed)
            {
                acceleration = -Brake;
                CurrentSpeed -= Brake*Time.deltaTime;
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
                BoostEnergy += BoostEnergyRegenerateRate*Time.deltaTime;
                if (BoostEnergy > MaxBoostEnergy)
                    BoostEnergy = MaxBoostEnergy;
            }
        }

        // Boosting
        if (TriggerBoost)
        {
            if (allowBoost && BoostEnergy > 0f)
            {
                if (CurrentSpeed < MaxSpeed)
                {
                    acceleration = BoostAcceleration;
                    CurrentSpeed += BoostAcceleration*Time.deltaTime;
                    CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
                }
                BoostEnergy -= BoostCost*Time.deltaTime;
                if (BoostEnergy < 0f)
                {
                    BoostEnergy = 0f;
                    allowBoost = false;
                }
                boostRegenerate = false;
                boostEnergyCooldown = BoostEnergyRegenerateDelay;
            }
        }

        // Idling
        if (!IsAccelerating && !IsBraking && !IsBoosting)
        {
            if (CurrentSpeed > IdleSpeed)
            {
                acceleration = -Brake;
                CurrentSpeed -= Brake*Time.deltaTime;
                CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
            }

            if (CurrentSpeed < IdleSpeed)
            {
                acceleration = Acceleration;
                CurrentSpeed += Acceleration*Time.deltaTime;
                CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
            }
        }

        rollSpeed += RollAcceleration*Mathf.Clamp(RollThrottle, -1, 1) * Time.deltaTime;

        if (Mathf.Abs(RollThrottle) < 0.01f)
        {
            rollSpeed = Mathf.Lerp(rollSpeed, 0f, 5f*Time.deltaTime);
        }

        // Turning
        var dYaw = YawThrottle*YawSpeed;
        var dPitch = PitchThotttle*PitchSpeed;
        var dRoll = -Mathf.Clamp(rollSpeed, -MaxRollSpeed, MaxRollSpeed) * Time.deltaTime;

        var targetRotation = transform.rotation*Quaternion.Euler(dPitch, dYaw, dRoll);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 20f*Time.deltaTime);

        _velocity = transform.forward*CurrentSpeed;
        _shiftable.Translate(_velocity*Time.deltaTime);

        var thrustAmount = acceleration/MaxSpeed;

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
            thrustAmount = 0.04f;
        }

        // Reduce flare brightness over distance from camera
        foreach (var thruster in Thrusters)
        {
            thruster.SetAmount(thrustAmount);
            thruster.UpdateFlare();
        }
    }
}
