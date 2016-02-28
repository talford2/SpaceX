using System.Collections.Generic;
using UnityEngine;

public class LineParticleDust : MonoBehaviour
{
	#region Public Variables

	public Transform Target;

	public int ParticleCount = 100;

	public float Radius = 10f;

	public Vector3 Velocity = Vector3.forward;

	public float ParticleLength = 1f;

	public float SpeedMultiplier = 1f;

	public Material ParticleMaterial;

	public float ParticleWidth = 1f;

	#endregion

	#region Private Variables

	private List<LineDustParticle> _particles;

	private Vector3 _lastPos;

	private Shiftable _shiftable;

	#endregion

	void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		if (_shiftable != null)
		{
			_shiftable.OnShift += _shiftable_OnShift;
		}

		// Creating prefab
		var particlePrefab = new GameObject();
		particlePrefab.transform.SetParent(transform);
		particlePrefab.transform.position = transform.position;
		var lineRender = particlePrefab.AddComponent<LineRenderer>();
		lineRender.useWorldSpace = false;
		particlePrefab.hideFlags = HideFlags.HideInHierarchy;

		// Create particle instances
		_particles = new List<LineDustParticle>();
		for (int i = 0; i < ParticleCount; i++)
		{
			var newParticle = Instantiate(particlePrefab);
			newParticle.transform.position = transform.position + Random.insideUnitSphere * Radius;
			newParticle.transform.forward = Velocity.normalized;
			newParticle.transform.SetParent(transform);

			var lineRenderer = newParticle.GetComponent<LineRenderer>();
			var linePositions = new List<Vector3>();
			linePositions.Add(Vector3.zero);
			linePositions.Add(Velocity * ParticleLength);
			lineRenderer.SetPositions(linePositions.ToArray());
			lineRenderer.material = ParticleMaterial;
			lineRenderer.SetWidth(ParticleWidth, ParticleWidth);
			newParticle.hideFlags = HideFlags.None;

			_particles.Add(new LineDustParticle
			{
				IsActive = true,
				Object = newParticle,
				LineRenderer = lineRenderer
			});
		}

		Destroy(lineRender);
	}

	private void _shiftable_OnShift(Shiftable sender, Vector3 delta)
	{
		_lastPos -= delta;
	}


	private Vector3 _targetPos = Vector3.zero;
	private Vector3 _particlePos = Vector3.zero;

	void Update()
	{
		_targetPos = Target.transform.position;

		Velocity = _lastPos - _targetPos;
		_lastPos = Target.transform.position;

		foreach (var particle in _particles)
		{
			_particlePos = particle.Object.transform.position;
			_particlePos += Velocity * Time.deltaTime;

			if ((_particlePos - _targetPos).sqrMagnitude > (Radius * Radius))
			{
				_particlePos = _targetPos + Random.onUnitSphere * Radius;
			}


			particle.Object.transform.position = _particlePos;

			particle.LineRenderer.SetPosition(1, Velocity.normalized * ParticleLength + SpeedMultiplier * Velocity);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Target.transform.position, Radius);

		Gizmos.color = new Color(0, 1, 0, 0.2f);
		Gizmos.DrawSphere(Target.transform.position, Radius);
	}

	private class LineDustParticle
	{
		public GameObject Object { get; set; }

		public LineRenderer LineRenderer { get; set; }

		public bool IsActive { get; set; }
	}
}