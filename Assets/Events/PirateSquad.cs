using System.Collections.Generic;
using UnityEngine;

public class PirateSquad : UniverseEvent
{
	private MapPin _mapPin;

	public List<Spawner> Spawners;

	public bool Randomize = false;
	public float Radius = 10f;

    private List<Fighter> _spawnedFighters;

	public override void Awake()
	{
		base.Awake();
		_mapPin = GetComponent<MapPin>();

        _spawnedFighters = new List<Fighter>();
		if (Randomize)
		{
			foreach (var s in Spawners)
			{
				s.transform.localPosition = Random.insideUnitSphere * Radius;
                s.OnSpawn += OnSpawnShip;
			}
		}
	}

	public override void Trigger()
	{
        if (Universe.Current.AllowEvents)
        {
            Universe.Current.AllowEvents = false;
            foreach (var spawner in Spawners)
            {
                spawner.Spawn(Random.Range(0f, 1f));
            }

            if (_mapPin != null)
                _mapPin.SetPinState(MapPin.MapPinState.Inactive);
            base.Trigger();
        }
	}

    private void OnSpawnShip(GameObject shipObject)
    {
        var fighter = shipObject.GetComponent<Fighter>();
        if (fighter != null)
        {
            _spawnedFighters.Add(fighter);
            fighter.VehicleInstance.Killable.OnDie += OnSpawnedDie;
        }
    }

    private void OnSpawnedDie(Killable sender)
    {
        if (_spawnedFighters.Count == 0)
        {
            Universe.Current.AllowEvents = true;
        }
    }
}
