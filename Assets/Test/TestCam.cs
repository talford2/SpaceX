using UnityEngine;
using System.Collections;

public class TestCam : MonoBehaviour
{
	public float Speed = 2f;

	public float FlickBackDistance = 10;

	public GameObject ParticleSystemTest;

	public bool UseLateUpdateForParticles = false;

	// Use this for initialization
	void Start()
	{

	}

	void Update()
	{
		if (!UseLateUpdateForParticles)
		{
			MoveParticleSystem();
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (UseLateUpdateForParticles)
		{
			MoveParticleSystem();
		}

		transform.position += Vector3.right * Speed * Time.deltaTime;
		if (transform.position.x > FlickBackDistance)
		{
			transform.position -= Vector3.right * FlickBackDistance;
		}
	}

	private void MoveParticleSystem()
	{
		ParticleSystemTest.transform.position += Vector3.right * Speed * Time.deltaTime;
		if (ParticleSystemTest.transform.position.x > FlickBackDistance)
		{
			ParticleSystemTest.transform.position -= Vector3.right * FlickBackDistance;
		}
	}
}
