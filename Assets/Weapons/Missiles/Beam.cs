using UnityEngine;

[RequireComponent(typeof(Shiftable))]
[RequireComponent(typeof(LineRenderer))]
public class Beam : Missile
{
	[Header("Beam")]
	public float MissileLength = 1000f;
	public float Radius = 0.5f;
	public float FireTime = 0.2f;

	private Shiftable _shiftable;
	private LineRenderer _lineRenderer;
	private ResourcePoolItem _resourcePoolItem;

	private Quaternion _dirRotate;
	private float _length;
	private float _fireCooldown;

	private int _hitMask;

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shiftable.OnShift += Shift;
		_lineRenderer = GetComponent<LineRenderer>();
		_length = MissileLength;
		_hitMask = ~LayerMask.GetMask("Distant", "Universe Background", "Environment");
	}

	public override void Initialize(GameObject owner, float damage)
	{
		if (_resourcePoolItem == null)
			_resourcePoolItem = GetComponent<ResourcePoolItem>();
		base.Initialize(owner, damage);
	}

	public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
	{
		base.Shoot(shootFrom, direction, initVelocity);
		_lineRenderer.enabled = true;

		_fireCooldown = FireTime;
		transform.position = FromReference.position;

		_dirRotate = Quaternion.FromToRotation(FromReference.forward, direction.normalized);

		transform.forward = direction;
		UpdateLineRenderer();
		_resourcePoolItem.IsAvailable = false;
	}

	public override void LiveUpdate()
	{
		if (_fireCooldown >= 0f)
		{
			_fireCooldown -= Time.deltaTime;
			if (_fireCooldown < 0f)
				Stop();
		}

		transform.position = FromReference.position;
		transform.forward = _dirRotate * FromReference.forward;

		_length = MissileLength;

		var shootRay = new Ray(transform.position, transform.forward);
		RaycastHit missileHit;
		if (Physics.SphereCast(shootRay, Radius, out missileHit, MissileLength, _hitMask))
		{
			var killable = missileHit.collider.GetComponentInParent<Killable>();
			if (killable != null)
			{
				killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
			}
			_length = missileHit.distance;
			PlaceHitEffects(missileHit.point, missileHit.normal, missileHit.collider.gameObject.transform);
		}
		UpdateLineRenderer();
	}

	private void UpdateLineRenderer()
	{
		var cooldownFraction = Mathf.Clamp01(_fireCooldown / FireTime);
		var diameter = 2f * Radius * cooldownFraction;
		_lineRenderer.SetWidth(diameter, diameter);
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, transform.position + transform.forward * _length);
	}

	private void Shift(Shiftable sender, Vector3 delta)
	{
		if (IsLive)
			UpdateLineRenderer();
	}

	public override void Stop()
	{
		base.Stop();
		_lineRenderer.enabled = false;
		if (_resourcePoolItem != null)
			_resourcePoolItem.IsAvailable = true;
	}
}
