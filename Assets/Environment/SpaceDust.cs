using System.Collections.Generic;
using UnityEngine;

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

	    CentreTransform = Universe.Current.ViewPort.transform;
	}

	public void LateUpdate()
	{
        var camTrans = Universe.Current.ViewPort.transform;

		foreach (var dustParticle in _particles)
		{
			var sqrMag = (CentreTransform.position - dustParticle.transform.position).sqrMagnitude;
			if (sqrMag > _radiusSqr)
			{
				dustParticle.transform.position = Random.onUnitSphere * Radius + CentreTransform.position;
			}
			dustParticle.transform.LookAt(dustParticle.transform.position + camTrans.forward, dustParticle.transform.up);
		}
	}
}
