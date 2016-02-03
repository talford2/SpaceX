using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public GameObject MissilePrefab;

	public MuzzleFlash MuzzlePrefab;

	public AudioSource FireSound;

	public float FireRate = 0.2f;
    public int MissilesPerShot = 2;
    public float MissileDamage;
    public bool MissilesConverge;
    public bool IsTargetLocking;
    public float TargetLockTime = 1.5f;
    public float TargetLockingMaxDistance = 2000f;

	private float _fireCooldown = 0f;
	private List<ShootPoint> _shootPoints;
    private int _shootPointIndex;
    private Vector3 _aimAt;

    private VelocityReference _velocityReference;

	public bool IsTriggered;

	public int MissilePoolCount = 20;

	public delegate void OnShootEvent();
	public OnShootEvent OnShoot;

	private int _curMissileIndex;
	private List<GameObject> _missileInstances;

    private GameObject _owner;
    private Team _ownerTeam;

    private Team _targetTeam;

    private Transform lockingTarget;
    private Transform lastLockingTarget;
    private Transform lockedTarget;
    private float lockingCooldown;
    private bool isLocked;

	public void Initialize(GameObject owner, List<ShootPoint> shootPoints, VelocityReference velocityReference, Team ownerTeam)
	{
	    _owner = owner;
		_curMissileIndex = 0;
		_missileInstances = new List<GameObject>();
		var missilesContainer = Utility.FindOrCreateContainer("Missiles");
		for (var i = 0; i < MissilePoolCount; i++)
		{
			var missileInstance = Utility.InstantiateInParent(MissilePrefab, missilesContainer);
			missileInstance.GetComponent<Missile>().Initialize(owner, MissileDamage);
			_missileInstances.Add(missileInstance);
		}
	    _shootPoints = shootPoints;
	    _shootPointIndex = 0;
	    _velocityReference = velocityReference;
	    _ownerTeam = ownerTeam;
	    _targetTeam = Targeting.GetEnemyTeam(_ownerTeam);
	}

	private void Update()
	{
		//Debug.LogFormat("{0} ===", IsTriggered);

	    if (!IsTargetLocking)
	    {
	        _fireCooldown -= Time.deltaTime;
	        if (IsTriggered && _fireCooldown < 0)
	        {
	            //Debug.Log("Shoot!");
	            _fireCooldown = FireRate;
	            Fire();
	        }
	    }
	    else
	    {
	        TargetLocking();
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

    private GameObject GetNextMissile()
    {
        var missile = _missileInstances[_curMissileIndex];
        _curMissileIndex++;
        if (_curMissileIndex >= _missileInstances.Count)
            _curMissileIndex = 0;
        return missile;
    }

    public void FireMissile(GameObject missile)
    {
        if (OnShoot != null)
            OnShoot();

        var _shootPoint = _shootPoints[_shootPointIndex];

        var direction = _aimAt - _shootPoint.transform.position;
        if (!MissilesConverge)
            direction += _shootPoint.transform.position - GetShootPointCentre();
        missile.GetComponent<Missile>().Shoot(_shootPoint.transform.position, direction, _velocityReference.Value);

        _shootPoint.Flash();
        FireSound.Play();

        _shootPointIndex++;
        if (_shootPointIndex >= _shootPoints.Count)
            _shootPointIndex = 0;
    }

	public void Fire()
	{
        for (var i = 0; i < MissilesPerShot; i++)
	    {
	        FireMissile(GetNextMissile());
	    }
	}

    private void ClearTargetLock()
    {
        isLocked = false;
        lastLockingTarget = null;
        lockedTarget = null;
    }

    private void TargetLocking()
    {
        var shootPointsCentre = GetShootPointCentre();
        if (lockingTarget != null)
        {
            var toLockingTarget = lockingTarget.position - shootPointsCentre;
            if (toLockingTarget.sqrMagnitude > TargetLockingMaxDistance*TargetLockingMaxDistance)
            {
                ClearTargetLock();
            }
        }
        else
        {
            ClearTargetLock();
        }
        if (lockedTarget != null)
        {
            var toLockedTarget = lockedTarget.position - shootPointsCentre;
            if (toLockedTarget.sqrMagnitude > TargetLockingMaxDistance*TargetLockingMaxDistance)
            {
                ClearTargetLock();
            }
        }
        else
        {
            ClearTargetLock();
        }

        if (!isLocked)
        {
            lockingTarget = null;
            if (IsTriggered)
            {
                var direction = _aimAt - shootPointsCentre;
                lockingTarget = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(_targetTeam), shootPointsCentre, direction, TargetLockingMaxDistance);
                if (lastLockingTarget == null)
                    lastLockingTarget = lockingTarget;
            }
            else
            {
                lastLockingTarget = null;
            }

            if (lastLockingTarget != null && lastLockingTarget == lockingTarget)
            {
                lockingCooldown -= Time.deltaTime;
                if (lockingCooldown < 0f)
                {
                    lockedTarget = lockingTarget;
                    isLocked = true;
                }
            }
            else
            {
                lockingCooldown = TargetLockTime;
            }
        }
        else
        {
            if (!IsTriggered)
            {
                var lockedVehicle = lockedTarget.GetComponent<Vehicle>();
                if (lockedVehicle != null)
                {
                    // Rough Extrapolation
                    var extrapolatePosition = Utility.GetVehicleExtrapolatedPosition(lockedVehicle, this, 0f);
                    SetAimAt(extrapolatePosition);
                }
                else
                {
                    SetAimAt(lockedTarget.position);
                }
                SetAimAt(GetShootPointCentre() + _velocityReference.Value);
                var nextMissile = GetNextMissile();
                nextMissile.GetComponent<Missile>().SetTarget(lockedTarget);
                FireMissile(nextMissile);
                ClearTargetLock();
            }
        }
    }

    public Transform GetLockingOnTarget()
    {
        return lockingTarget;
    }

    public Transform GetLockedOnTarget()
    {
        return lockedTarget;
    }
}
