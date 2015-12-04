using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public Fighter FighterPrefab;

	private Fighter _fighterInst;

	public Shiftable Shifter;

	public bool SpawnOnAwake = false;

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
		StartCoroutine(DoSpawn(delay));
	}

	private IEnumerator DoSpawn(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (!HasSpawned)
		{
			_fighterInst = Instantiate<Fighter>(FighterPrefab);

			var universePosition = new UniversePosition(Shifter.UniverseCellIndex, Shifter.CellLocalPosition + transform.position);
			_fighterInst.SpawnVehicle(_fighterInst.VehiclePrefab, universePosition);

			var s = _fighterInst.VehicleInstance.gameObject.AddComponent<WarpEffect>();
			s.Timeout = 1f;
			s.Distance = 30f;

			HasSpawned = true;
		}
	}
}
