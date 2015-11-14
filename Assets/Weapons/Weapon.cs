using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public GameObject MissilePrefab;

	public MuzzleFlash MuzzlePrefab;

	public AudioSource FireSound;

	public float FireRate = 0.2f;

	private float _fireCooldown = 0f;
	private Transform _shootPoint;
	private Vector3 _initVelocity;

	public bool IsTriggered;

	public int MissilePoolCount = 20;

	public delegate void OnShootEvent();
	public OnShootEvent OnShoot;

	private int _curMissileIndex;
	private List<GameObject> _missileInstances;

	private MuzzleFlash _muzzleInstance;

	public void Initialize(GameObject owner)
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

	    _muzzleInstance = Utility.InstantiateInParent(MuzzlePrefab.gameObject, transform).GetComponent<MuzzleFlash>();
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

	public void SetShootPoint(Transform shootPoint, Vector3 initVelocity)
	{
		_shootPoint = shootPoint;
		_initVelocity = initVelocity;

		_muzzleInstance.transform.position = _shootPoint.position;
		_muzzleInstance.transform.forward = _shootPoint.forward;
	}

	public void Fire()
	{
		var missile = _missileInstances[_curMissileIndex];

		if (OnShoot != null)
			OnShoot();

		missile.transform.position = _shootPoint.position;
		missile.transform.rotation = _shootPoint.rotation;
		missile.GetComponent<Shiftable>().UniverseCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
		missile.GetComponent<Missile>().Shoot(_shootPoint.position, _shootPoint.forward, _initVelocity);

		_muzzleInstance.Flash();
		FireSound.Play();

		_curMissileIndex++;
		if (_curMissileIndex >= _missileInstances.Count)
			_curMissileIndex = 0;
	}
}
