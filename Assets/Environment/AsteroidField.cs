using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
	public int AsteroidPoolCount = 1000;

	public Transform CentreTransform;

	public float FieldRadius = 300f;

	public float FieldOfView = 60f;

	public GameObject AsteroidPrefab;

	public float MinSize = 0.3f;

	public float MaxSize = 3.0f;

	public float MinRotationSpeed = 20f;

	public float MaxRotationSpeed = 60f;

	private float _halfFov;



	private List<AsteroidObj> _asteroids;

	private void Awake()
	{
		_asteroids = new List<AsteroidObj>();

		_halfFov = FieldOfView / 2f;
		for (var i = 0; i < AsteroidPoolCount; i++)
		{
			var inst = Instantiate(AsteroidPrefab);

			inst.transform.position = Random.insideUnitSphere * FieldRadius + CentreTransform.position;
			inst.transform.rotation = Random.rotation;
			inst.GetComponent<ScaleOnEnable>().ResetToZero(Random.Range(MinSize, MaxSize));

			inst.transform.parent = transform;

			inst.GetComponent<Killable>().OnDie += AsteroidField_OnDie;

			var levelOfDetail = inst.GetComponent<LevelOfDetail>();
			levelOfDetail.OnExitLargestDistance += MakeAvailable;

			_asteroids.Add(new AsteroidObj { Asteroid = inst, Rotation = RandomEuler(Random.Range(MinRotationSpeed, MaxRotationSpeed)) });
		}
	}

	private void Update()
	{
		foreach (var asteroid in _asteroids)
		{
			asteroid.Asteroid.transform.rotation *= Quaternion.Euler(asteroid.Rotation * Time.deltaTime);
		}
	}

	private void AsteroidField_OnDie(Killable sender, GameObject attacker)
	{
		// TODO: This isn't perfect, whenever an asteroid is destroyed a new one will be respawned,
		// really the asteroid should just be hidden and reset when it falls out of the range
		MakeAvailable(sender.gameObject);
	}

	private void MakeAvailable(GameObject instance)
	{
		var randomInFront = Quaternion.Euler(Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov)) * Universe.Current.ViewPort.transform.forward;
		instance.transform.position = randomInFront * FieldRadius + CentreTransform.position;
		instance.GetComponent<ScaleOnEnable>().ResetToZero(Random.Range(MinSize, MaxSize));

		var killable = instance.GetComponent<Killable>();
		killable.IsAlive = true;
		killable.Health = killable.MaxHealth;
	}

	private Vector3 RandomEuler(float magnitude)
	{
		var halfMagnitude = magnitude / 2f;
		return new Vector3(Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude));
	}

	public class AsteroidObj
	{
		public GameObject Asteroid;

		public Vector3 Rotation;
	}
}
