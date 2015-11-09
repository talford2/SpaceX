using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceDust : MonoBehaviour
{
	public GameObject DustParticlePrefab;

	private List<GameObject> _particles;

	public Transform CentreTransform;

	public int DustParticleCount = 100;

	public float Radius = 200f;

	private float _radiusSqr;

	public float Opacity = 1;

	public void Start()
	{
		_radiusSqr = Radius * Radius;
		_particles = new List<GameObject>();

		for (var i = 0; i < DustParticleCount; i++)
		{
			var inst = Instantiate(DustParticlePrefab);

			inst.transform.position = Random.insideUnitSphere * Radius;
			inst.transform.rotation = Random.rotation;

			inst.transform.parent = this.transform;
			_particles.Add(inst);
		}

		CentreTransform = FollowCamera.Current.transform;
	}

	public void LateUpdate()
	{
		//var index = 0;
		foreach (var dustParticle in _particles)
		{
			//dustParticle.GetComponent<Renderer>().material.color = Color.red;

			//var sqrMag = (transform.position - CentreTransform.position).sqrMagnitude;
			var sqrMag = (CentreTransform.position - dustParticle.transform.position).sqrMagnitude;

			if (sqrMag > _radiusSqr)
			{
				dustParticle.transform.position = Random.onUnitSphere * Radius + CentreTransform.position;
				dustParticle.transform.rotation = Random.rotation;
			}
			//index++;
			//Random.seed = index;
			dustParticle.transform.LookAt(CentreTransform);
		}
	}
}
