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
		foreach (var dustParticle in _particles)
		{
			var sqrMag = (CentreTransform.position - dustParticle.transform.position).sqrMagnitude;

			if (sqrMag > _radiusSqr)
			{
				dustParticle.transform.position = Random.onUnitSphere * Radius + CentreTransform.position;
				dustParticle.transform.rotation = Random.rotation;
			}

			//dustParticle.transform.LookAt(CentreTransform);
			//dustParticle.transform.Rotate(Vector3.up, 180f);

			//dustParticle.transform.forward = FollowCamera.Current.transform.forward * -1;

			//var camTrans = FollowCamera.Current.transform;
			//dustParticle.transform.LookAt(transform.position + camTrans.rotation * Vector3.forward, camTrans.rotation * Vector3.up);

			dustParticle.transform.forward = FollowCamera.Current.transform.forward * -1;
		}
	}
}
