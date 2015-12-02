using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PirateSquad : UniverseEvent
{
	public List<Spawner> Spawners;

	public override void Initialise()
	{
		foreach (var spawner in Spawners)
		{
			spawner.Spawn();
		}

		base.Initialise();
		//Debug.Log("Pop!");
	}
}
