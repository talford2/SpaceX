﻿using System.Collections.Generic;
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

    public float RollSpeed = 100f;

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

	public List<Transform> ShootPoints;

    [Header("Other")]
    public List<LensFlare> ThrusterFlares;

	private Shiftable _shiftable;

	private int _shootPointIndex;

	private Weapon _weaponInstance;

    private Vector3 _velocity;

    private readonly float _aimDistance = 1000f;
    private readonly float _yawClamp = 2f;
    private readonly float _pitchClamp = 2f;

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
        return ShootPoints.Aggregate(Vector3.zero, (current, shootPoint) => current + shootPoint.position)/ShootPoints.Count;
    }

    public Vector3 GetAimPosition()
    {
        return GetShootPointCentre() + _aimDistance*transform.forward;
    }

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shootPointIndex = 0;

		_weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_weaponInstance.Initialize(gameObject);
		_weaponInstance.OnShoot += OnShoot;

	    allowBoost = true;
	}

	private void OnShoot()
	{
		_weaponInstance.SetShootPoint(ShootPoints[_shootPointIndex], _velocity);
		_shootPointIndex++;
		if (_shootPointIndex >= ShootPoints.Count)
			_shootPointIndex = 0;
	}

    private void Update()
    {
        if (!TriggerBoost)
        {
            // Accelerating
            if (TriggerAccelerate && CurrentSpeed < MaxSpeed)
            {
                CurrentSpeed += Acceleration*Time.deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
            }

            // Braking
            if (TriggerBrake && CurrentSpeed > MinSpeed)
            {
                CurrentSpeed -= Brake*Time.deltaTime;
                CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
            }

            if (!allowBoost)
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
        if (TriggerBoost && CurrentSpeed < MaxSpeed)
        {
            if (allowBoost && BoostEnergy > 0f)
            {
                CurrentSpeed += BoostAcceleration * Time.deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);

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
        if (!TriggerAccelerate && !TriggerBrake)
        {
            if (CurrentSpeed > IdleSpeed)
            {
                CurrentSpeed -= Brake*Time.deltaTime;
                CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
            }

            if (CurrentSpeed < IdleSpeed)
            {
                CurrentSpeed += Acceleration*Time.deltaTime;
                CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
            }
        }

        // Turning
        var dYaw = Mathf.Clamp(YawThrottle, -_yawClamp, _yawClamp)*YawSpeed;
        var dPitch = Mathf.Clamp(PitchThotttle, -_pitchClamp, _pitchClamp)*PitchSpeed;
        var dRoll = Mathf.Clamp(RollThrottle, -1f, 1f)*-RollSpeed;

        var targetRotation = transform.rotation * Quaternion.Euler(dPitch * Time.deltaTime, dYaw * Time.deltaTime, dRoll * Time.deltaTime);

        //transform.rotation *= Quaternion.Euler(dPitch*Time.deltaTime, dYaw*Time.deltaTime, dRoll*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 20f*Time.deltaTime);

        _velocity = transform.forward*CurrentSpeed;
        _shiftable.Translate(_velocity*Time.deltaTime);

        // Reduce flare brightness over distance from camera
        foreach (var thruster in ThrusterFlares)
        {
            var toCamera = Universe.Current.ViewPort.Shiftable.GetWorldPosition() - Shiftable.GetWorldPosition();
            const float theFactor = 2000f;
            var capFlareBright = maxFlareBrightness / Mathf.Max(toCamera.sqrMagnitude / theFactor, 1f);
            thruster.brightness = 1f * capFlareBright;
        }
    }
}
