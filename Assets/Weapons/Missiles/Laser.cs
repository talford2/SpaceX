using UnityEngine;

public class Laser : Missile
{
	[Header("Laser")]
	public float MissileLength = 6f;

	public LineRenderer Tracer;
	private Shiftable _shiftable;
	private ResourcePoolItem _resourcePoolItem;
	private bool _hasHit;

	private Vector3 _shootFrom;
	private Vector3 _initVelocity;
	private float _initSpeed;
	private Vector3 _hitPosition;

	private Vector3 _observationPosition;
	private float _rayCheckMaxDistSquared = 25000000f; // 5,000
	private float _stopDistanceSquared = 100000000f; // 10,000

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shiftable.OnShift += Shift;
		if (Tracer != null)
		{
			Tracer.useWorldSpace = true;
		}
	}

	public override void Initialize(GameObject owner, float damage)
	{
		if (_resourcePoolItem == null)
			_resourcePoolItem = GetComponent<ResourcePoolItem>();
		base.Initialize(owner, damage);
	}

	private Ray missileRay = new Ray();
	private RaycastHit missileHit;

	// Perfromance variables
	private float _displacement;
	private float _toOberverSquared;
	private Killable _killable;
	private int _mask = ~LayerMask.GetMask("Distant", "Universe Background", "Environment", "Player");

	public override void LiveUpdate()
	{
		_displacement = (_initSpeed + MissileSpeed) * Time.deltaTime;
		_observationPosition = Universe.Current.ViewPort.transform.position;
		_toOberverSquared = (transform.position - _observationPosition).sqrMagnitude;
		if (_toOberverSquared < _rayCheckMaxDistSquared)
		{
			if (!_hasHit)
			{
				//missileRay = new Ray(transform.position, transform.forward);

				missileRay.origin = transform.position;
				missileRay.direction = transform.forward;

				//RaycastHit missileHit;
				if (Physics.Raycast(missileRay, out missileHit, _displacement, _mask))
				{
					if (missileHit.collider.gameObject != Owner)
					{
						_killable = missileHit.collider.GetComponentInParent<Killable>();
						if (_killable != null)
						{
							_killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
						}
						_hasHit = true;
						_hitPosition = missileHit.point;
						PlaceHitEffects(missileHit.point, missileHit.normal, missileHit.collider.gameObject.transform);
					}
				}
			}
		}
		else
		{
			if (_toOberverSquared > _stopDistanceSquared)
			{
				Stop();
			}
		}
		_shiftable.Translate(transform.forward * _displacement);
	}

	public void LateUpdate()
	{
		if (IsLive)
		{
			UpdateLineRenderer();
		}
	}

	// Performance variables
	private Vector3 _headPosition;
	private Vector3 _tailPosition;
	private float _tailDotProd;
	private float _headHitDotProd;
	private float _tailHitDotProd;

	public void UpdateLineRenderer()
	{
		_headPosition = transform.position;
		_tailPosition = _headPosition - transform.forward * MissileLength;

		_tailDotProd = Vector3.Dot(_tailPosition - _shootFrom, transform.forward);
		_headHitDotProd = Vector3.Dot(_headPosition - _hitPosition, transform.forward);

		if (_hasHit && _headHitDotProd > 0f)
		{
			_headPosition = _hitPosition;
			_tailHitDotProd = Vector3.Dot(_tailPosition - _hitPosition, transform.forward);
			if (_tailHitDotProd > 0f)
			{
				Stop();
				return;
			}
		}

		if (_tailDotProd < 0f)
		{
			_tailPosition = _shootFrom;
		}

		Tracer.SetPosition(0, _headPosition);
		Tracer.SetPosition(1, _tailPosition);
	}

	public override void Stop()
	{
		base.Stop();
		Tracer.enabled = false;
		_hasHit = false;
		if (_resourcePoolItem != null)
			_resourcePoolItem.IsAvailable = true;
		//Universe.Current.ShiftableItems.Remove(_shiftable);
	}

	public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
	{
		base.Shoot(shootFrom, direction, initVelocity);
		Tracer.enabled = true;
		_shootFrom = shootFrom;
		_initVelocity = initVelocity;
		_initSpeed = _initVelocity.magnitude;
		_hasHit = false;
		transform.position = _shootFrom;
		transform.position += initVelocity * Time.deltaTime;
		transform.forward = direction;
		UpdateLineRenderer();
		_resourcePoolItem.IsAvailable = false;
		//Universe.Current.ShiftableItems.Add(_shiftable);
	}

	private void Shift(Shiftable sender, Vector3 delta)
	{
		if (IsLive)
		{
			_shootFrom -= delta;
			_hitPosition -= delta;
			UpdateLineRenderer();
		}
	}
}
