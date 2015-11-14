using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public GameObject MissilePrefab;

	public MuzzleFlash MuzzlePrefab;

	public AudioSource FireSound;

	public float FireRate = 0.2f;

	private float _fireCooldown = 0f;
	private ShootPoint _shootPoint;
	private Vector3 _initVelocity;

	public bool IsTriggered;

	public int MissilePoolCount = 20;

	public delegate void OnShootEvent();
	public OnShootEvent OnShoot;

	private int _curMissileIndex;
	private List<GameObject> _missileInstances;

	//private MuzzleFlash _muzzleInstance;

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

	public void SetShootPoint(ShootPoint shootPoint, Vector3 initVelocity)
	{
		_shootPoint = shootPoint;
		_initVelocity = initVelocity;
	}

	public void Fire()
	{
		var missile = _missileInstances[_curMissileIndex];

		if (OnShoot != null)
			OnShoot();

		missile.transform.position = _shootPoint.transform.position;
		missile.transform.rotation = _shootPoint.transform.rotation;
		missile.GetComponent<Shiftable>().UniverseCellIndex = PlayerController.Current.VehicleInstance.Shiftable.UniverseCellIndex;
		missile.GetComponent<Missile>().Shoot(_shootPoint.transform.position, _shootPoint.transform.forward, _initVelocity);

		_shootPoint.Flash();
		FireSound.Play();

		_curMissileIndex++;
		if (_curMissileIndex >= _missileInstances.Count)
			_curMissileIndex = 0;
	}
}
