using System;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public Fighter FighterPrefab;

	private Fighter _fighterInst;

	public Shiftable Shifter;

	public GameObject SpawnEffect;

	public bool SpawnOnAwake = false;
	public bool AddWarpEffect = true;

	private bool _hasSpawned = false;

    public delegate void OnSpawnerSpawn(GameObject spawnedObject);
    public OnSpawnerSpawn OnSpawn;

	private void Awake()
	{
		if (SpawnOnAwake)
		{
			Spawn();
		}
	}

	public void Spawn(float delay = 0)
	{
		StartCoroutine(DoSpawn(delay, null, null));
	}

	public void Spawn(float delay, Action callback)
	{
		StartCoroutine(DoSpawn(delay, callback, null));
	}

    public void Spawn(float delay, Transform targetTransform)
    {
        StartCoroutine(DoSpawn(delay, null, targetTransform));
    }

	public void Reset()
	{
		_hasSpawned = false;
	}

	private WarpEffect _warpEffect;
	private IEnumerator DoSpawn(float delay, Action callback, Transform targetTransform)
	{
		yield return new WaitForSeconds(delay);

		if (!_hasSpawned)
		{
			_fighterInst = Instantiate<Fighter>(FighterPrefab);
			_fighterInst.SpawnVehicle(_fighterInst.gameObject, _fighterInst.VehiclePrefab, Universe.Current.GetUniversePosition(transform.position), transform.rotation);
            if (targetTransform != null)
                _fighterInst.SetTarget(targetTransform);
			if (AddWarpEffect)
			{
				_warpEffect = _fighterInst.VehicleInstance.gameObject.AddComponent<WarpEffect>();
				_warpEffect.transform.position = transform.position;
				_warpEffect.transform.forward = transform.forward;

				_warpEffect.WarpEffectPrefab = SpawnEffect;
				_warpEffect.Timeout = 0.4f;
				_warpEffect.Distance = 20f;
			}
			_hasSpawned = true;
			if (callback != null)
				callback();
            if (OnSpawn != null)
                OnSpawn(_fighterInst.gameObject);
		}
	}
}
