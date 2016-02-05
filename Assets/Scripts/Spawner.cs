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

	private bool HasSpawned = false;

	private float _delayTime = 0f;

	private void Awake()
	{
		if (SpawnOnAwake)
		{
			Spawn();
		}
	}

	public void Spawn(float delay = 0)
	{
		StartCoroutine(DoSpawn(delay, null));
	}

    public void Spawn(float delay, Action callback)
    {
        StartCoroutine(DoSpawn(delay, callback));
    }

    public void Reset()
    {
        HasSpawned = false;
    }

	private IEnumerator DoSpawn(float delay, Action callback)
	{
		yield return new WaitForSeconds(delay);

		if (!HasSpawned)
		{
			_fighterInst = Instantiate<Fighter>(FighterPrefab);
            _fighterInst.SpawnVehicle(_fighterInst.VehiclePrefab, Universe.Current.GetUniversePosition(transform.position), transform.rotation);

		    if (AddWarpEffect)
		    {
		        var s = _fighterInst.VehicleInstance.gameObject.AddComponent<WarpEffect>();
		        s.transform.position = transform.position;
		        s.transform.forward = transform.forward;

		        s.WarpEffectPrefab = SpawnEffect;
		        s.Timeout = 0.4f;
		        s.Distance = 20f;
		    }
		    HasSpawned = true;
		    if (callback != null)
		        callback();
		}
	}
}
