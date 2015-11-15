using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public GameObject MissilePrefab;

	public MuzzleFlash MuzzlePrefab;

	public AudioSource FireSound;

	public float FireRate = 0.2f;
    public int MissilesPerShot = 2;

	private float _fireCooldown = 0f;
	private List<ShootPoint> _shootPoints;
    private int _shootPointIndex;

	private Vector3 _initVelocity;

	public bool IsTriggered;

	public int MissilePoolCount = 20;

	public delegate void OnShootEvent();
	public OnShootEvent OnShoot;

	private int _curMissileIndex;
	private List<GameObject> _missileInstances;

	public void Initialize(GameObject owner, List<ShootPoint> shootPoints)
	{
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
	}

	private void Update()
	{
		_fireCooldown -= Time.deltaTime;

		//Debug.LogFormat("{0} ===", IsTriggered);

		if (IsTriggered && _fireCooldown < 0)
		{
			//Debug.Log("Shoot!");
			_fireCooldown = FireRate;
			Fire();
		}
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
	        missile.transform.rotation = _shootPoint.transform.rotation;
	        missile.GetComponent<Shiftable>().UniverseCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
	        missile.GetComponent<Missile>().Shoot(_shootPoint.transform.position, _shootPoint.transform.forward, _initVelocity);

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
}
