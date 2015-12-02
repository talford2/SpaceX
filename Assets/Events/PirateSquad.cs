using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PirateSquad : UniverseEvent
{
	private Material _testMat;

	public Renderer Sphere;

	public List<Spawner> Spawners;

	public override void Awake()
	{
		base.Awake();
		_testMat = Sphere.material;
		_testMat.SetColor("_Color", Color.yellow);
	}

	public override void Initialise()
	{
		foreach (var spawner in Spawners)
		{
			spawner.Spawn();
		}
		_testMat.SetColor("_Color", Color.red);
		base.Initialise();
	}
}
