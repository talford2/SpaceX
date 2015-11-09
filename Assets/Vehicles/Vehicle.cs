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

	public float CurrentSpeed = 0f;

	public float PitchSpeed = 5f;

	public float YawSpeed = 5f;


	[Header("Control Settings")]
	public bool IsAccelerating = false;

	public bool IsBraking = false;

	public float YawThrottle = 0f;

	public float PitchThotttle = 0f;

	[Header("Weapon")]
	public Weapon WeaponPrefab;

	public List<Transform> ShootPoints;

	private Shiftable _shiftable;

	private int _shootPointIndex;

	private Weapon _weaponInstance;

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

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shootPointIndex = 0;

		_weaponInstance = Utility.InstantiateInParent(WeaponPrefab.gameObject, transform).GetComponent<Weapon>();
		_weaponInstance.Initialize(gameObject);
		_weaponInstance.OnShoot += OnShoot;
	}

	private void OnShoot()
	{
		_weaponInstance.SetShootPoint(ShootPoints[_shootPointIndex]);
		_shootPointIndex++;
		if (_shootPointIndex >= ShootPoints.Count)
			_shootPointIndex = 0;
	}

	private void Update()
	{
		// Accelerating
		if (IsAccelerating && CurrentSpeed < MaxSpeed)
		{
			CurrentSpeed += Acceleration * Time.deltaTime;
			CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
		}

		// Braking
		if (IsBraking && CurrentSpeed > MinSpeed)
		{
			CurrentSpeed -= Brake * Time.deltaTime;
			CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
		}

		// Idling
		if (!IsAccelerating && !IsBraking)
		{
			if (CurrentSpeed > IdleSpeed)
			{
				CurrentSpeed -= Brake * Time.deltaTime;
				CurrentSpeed = Mathf.Max(IdleSpeed, CurrentSpeed);
			}

			if (CurrentSpeed < IdleSpeed)
			{
				CurrentSpeed += Acceleration * Time.deltaTime;
				CurrentSpeed = Mathf.Min(IdleSpeed, CurrentSpeed);
			}
		}

		// Turning
		transform.rotation *= Quaternion.Euler(0, YawThrottle * YawSpeed * Time.deltaTime, 0);
		transform.rotation *= Quaternion.Euler(PitchThotttle * PitchSpeed * Time.deltaTime, 0, 0);

		//transform.position += transform.forward * CurrentSpeed * Time.deltaTime;
		_shiftable.Translate(transform.forward * CurrentSpeed * Time.deltaTime);
	}
}
