using System.Collections.Generic;
using UnityEngine;

public class PirateSquad : UniverseEvent
{
	private MapPin _mapPin;
	
	public List<Spawner> Spawners;

	public override void Awake()
	{
		base.Awake();
		_mapPin = GetComponent<MapPin>();
	}

	public override void Initialise()
	{
		foreach (var spawner in Spawners)
		{
			spawner.Spawn(Random.Range(0f, 1f));
		}
		
		if (_mapPin != null)
			_mapPin.SetPinState(MapPin.MapPinState.Inactive);
		base.Initialise();
	}
}
