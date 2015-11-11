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

    public float RollSpeed = 100f;


	[Header("Control Settings")]
	public bool IsAccelerating = false;

	public bool IsBraking = false;

	public float YawThrottle = 0f;

	public float PitchThotttle = 0f;

    public float RollThrottle = 0f;

	[Header("Weapon")]
	public Weapon WeaponPrefab;

	public List<Transform> ShootPoints;

	private Shiftable _shiftable;

	private int _shootPointIndex;

	private Weapon _weaponInstance;

    private Vector3 _velocity;

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
		_weaponInstance.SetShootPoint(ShootPoints[_shootPointIndex], _velocity);
		_shootPointIndex++;
		if (_shootPointIndex >= ShootPoints.Count)
			_shootPointIndex = 0;
	}

    private void Update()
    {
        // Accelerating
        if (IsAccelerating && CurrentSpeed < MaxSpeed)
        {
            CurrentSpeed += Acceleration*Time.deltaTime;
            CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
        }

        // Braking
        if (IsBraking && CurrentSpeed > MinSpeed)
        {
            CurrentSpeed -= Brake*Time.deltaTime;
            CurrentSpeed = Mathf.Max(CurrentSpeed, MinSpeed);
        }

        // Idling
        if (!IsAccelerating && !IsBraking)
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
        transform.rotation *= Quaternion.AngleAxis(Mathf.Clamp(YawThrottle, -1f, 1f)*YawSpeed*Time.deltaTime, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(Mathf.Clamp(PitchThotttle, -1f, 1f)*PitchSpeed*Time.deltaTime, Vector3.right);
        transform.rotation *= Quaternion.AngleAxis(Mathf.Clamp(RollThrottle, -1f, 1f)*-RollSpeed*Time.deltaTime, Vector3.forward);

        //transform.position += transform.forward * CurrentSpeed * Time.deltaTime;
        _velocity = transform.forward*CurrentSpeed;
        _shiftable.Translate(_velocity*Time.deltaTime);
    }
}
