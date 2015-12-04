using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdvancedParticlewSystem : MonoBehaviour
{
	#region Public Variables

	public float EmissionRate = 10f;

	public float MinLife = 0.5f;

	public float MaxLife = 2f;

	public float Duration = 1f;

	public float MinSpeed = 5f;

	public float MaxSpeed = 10f;

	public AnimationCurve SpeedOverTime = AnimationCurve.Linear(0, 1, 1, 1);

	public int MaximumParticles = 100;

	public bool PlayOnAwake = true;

	public float ParticleLength = 2f;

	public float ParticleWidth = 1f;

	public Material ParticleMaterial;

	public Gradient ColorOverTime;

	#endregion

	#region Private Variables

	private bool IsPlaying = false;
	
	private float _durationCooldown = 0;

	private float _emissionCooldown = 0;

	private List<AdvancedParticle> _particles;

	#endregion

	void Awake()
	{
		// Creating prefab
		var particlePrefab = new GameObject();
		particlePrefab.transform.SetParent(transform);
		particlePrefab.transform.position = transform.position;
		var lineRender = (LineRenderer)particlePrefab.AddComponent(typeof(LineRenderer));
		lineRender.useWorldSpace = false;
		particlePrefab.hideFlags = HideFlags.HideInHierarchy;

		// Create particle instances
		_particles = new List<AdvancedParticle>();
		for (int i = 0; i < MaximumParticles; i++)
		{
			var newParticle = Instantiate(particlePrefab);
			newParticle.transform.position = transform.position;
			newParticle.transform.localRotation = Random.rotation;
			newParticle.transform.SetParent(transform);

			var lr = newParticle.GetComponent<LineRenderer>();

			var v3List = new List<Vector3>();

			v3List.Add(Vector3.zero);
			v3List.Add(Vector3.forward * ParticleLength);

			lr.SetPositions(v3List.ToArray());


			lr.material = ParticleMaterial;
			lr.SetWidth(ParticleWidth, ParticleWidth);
			newParticle.hideFlags = HideFlags.None;

			var life = Random.Range(MinLife, MaxLife);

			_particles.Add(new AdvancedParticle
			{
				Object = newParticle,
				LineRenderer = lr,
				Speed = Random.Range(MinSpeed, MaxSpeed),
				Life = life,
				LifeTime = life,
				IsActive = true
			});
		}
	}

	public void Start()
	{
		if (PlayOnAwake)
		{
			StartEmission();
		}
	}

	public void StartEmission()
	{
		IsPlaying = true;
		_durationCooldown = Duration;
		_emissionCooldown = 1;
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			StartEmission();
		}
		
		if (IsPlaying)
		{
			foreach (var particle in _particles)
			{
				if (particle.IsActive)
				{
					var frac = particle.Life / particle.LifeTime;

					frac = 1 - Mathf.Max(frac, 0);

					var eval = SpeedOverTime.Evaluate(frac);
					//Debug.LogFormat("Frac: {0}, Life: {1}, Eval: {2}", frac, particle.Life, eval);

					particle.Object.transform.localPosition += particle.Object.transform.forward * particle.Speed * eval * Time.deltaTime;

					particle.LineRenderer.material.SetColor("_Color", ColorOverTime.Evaluate(frac));

					if (particle.Life < 0)
					{
						particle.IsActive = false;
					}
					particle.Life -= Time.deltaTime;
				}
			}
		}
	}

	private class AdvancedParticle
	{
		public GameObject Object { get; set; }

		public LineRenderer LineRenderer { get; set; }

		public float LifeTime { get; set; }

		public float Life { get; set; }

		public float Speed { get; set; }

		public bool IsActive { get; set; }
	}
}
