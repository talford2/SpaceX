using System.Collections.Generic;
using UnityEngine;

public class PirateSquad : UniverseEvent
{
	private MapPin _mapPin;

	public List<Spawner> Spawners;

	public bool Randomize = false;
	public float Radius = 10f;

	public override void Awake()
	{
		base.Awake();
		_mapPin = GetComponent<MapPin>();

		if (Randomize)
		{
			foreach (var s in Spawners)
			{
				s.transform.localPosition = Random.insideUnitSphere * Radius;
			}
		}
	}

	public override void Trigger()
	{
		foreach (var spawner in Spawners)
		{
			spawner.Spawn(Random.Range(0f, 1f));
		}

		if (_mapPin != null)
			_mapPin.SetPinState(MapPin.MapPinState.Inactive);
		base.Trigger();
	}
}
