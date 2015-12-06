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
			missileInstance.GetComponent<Missile>().SetOwner(owner);
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

	public void Fire()
	{
        for (var i = 0; i < MissilesPerShot; i++)
	    {
	        var missile = _missileInstances[_curMissileIndex];

	        if (OnShoot != null)
	            OnShoot();

	        var _shootPoint = _shootPoints[_shootPointIndex];

	        missile.transform.position = _shootPoint.transform.position;
	        var direction = _aimAt - _shootPoint.transform.position;
	        if (!MissilesConverge)
	            direction += _shootPoint.transform.position - GetShootPointCentre();
	        missile.transform.forward = direction;
	        //missile.GetComponent<Shiftable>().UniverseCellIndex = _owner.GetComponent<Vehicle>().Shiftable.UniverseCellIndex;
	        missile.GetComponent<Missile>().Shoot(_shootPoint.transform.position, direction, _velocityReference.Value);

	        _shootPoint.Flash();
	        FireSound.Play();

	        _curMissileIndex++;
	        if (_curMissileIndex >= _missileInstances.Count)
	            _curMissileIndex = 0;

	        _shootPointIndex++;
	        if (_shootPointIndex >= _shootPoints.Count)
	            _shootPointIndex = 0;
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
        if (lockedTarget != null)
        {
            var toLockedTarget = lockedTarget.position - shootPointsCentre;
            if (toLockedTarget.sqrMagnitude > TargetLockingMaxDistance*TargetLockingMaxDistance)
            {
                ClearTargetLock();
            }
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
                    var extrapolatePosition = Utility.GetVehicleExtrapolatedPosition(lockedVehicle, this);
                    SetAimAt(extrapolatePosition);
                }
                else
                {
                    SetAimAt(lockedTarget.position);
                }
                Fire();
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
