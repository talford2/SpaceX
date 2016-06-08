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

		_mask = ~LayerMask.GetMask("Distant", "Universe Background", "Environment", "Player");
	}

	public override void Initialize(GameObject owner, float damage)
	{
		if (_resourcePoolItem == null)
			_resourcePoolItem = GetComponent<ResourcePoolItem>();
		base.Initialize(owner, damage);
	}

	private Killable _killable;

	private int _mask;

	public override void LiveUpdate()
	{
		var displacement = (_initSpeed + MissileSpeed) * Time.deltaTime;
		var observationPosition = Universe.Current.ViewPort.transform.position;
		var toOberverSquared = (transform.position - observationPosition).sqrMagnitude;
		if (toOberverSquared < _rayCheckMaxDistSquared)
		{
			if (!_hasHit)
			{
				var missileRay = new Ray(transform.position, transform.forward);

				RaycastHit missileHit;

				if (Physics.Raycast(missileRay, out missileHit, displacement, _mask))
				{
					if (missileHit.collider.gameObject != Owner)
					{
						_killable = missileHit.collider.GetComponentInParent<Killable>();
                        if (_killable != null)
                        {
                            if (PlayerController.Current.VehicleInstance != null && Owner == PlayerController.Current.VehicleInstance.gameObject)
                                HeadsUpDisplay.Current.TriggerCrosshairPulse();
                            _killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
                        }
                        var rBody = missileHit.collider.GetComponentInParent<Rigidbody>();
                        if (rBody != null)
                            rBody.AddForceAtPosition(MissileForce * missileRay.direction, missileHit.point, ForceMode.Impulse);
						_hasHit = true;
						_hitPosition = missileHit.point;
						PlaceHitEffects(missileHit.point, missileHit.normal, missileHit.collider.gameObject.transform);
					}
				}
			}
		}
		else
		{
			if (toOberverSquared > _stopDistanceSquared)
			{
				Stop();
			}
		}
		_shiftable.Translate(transform.forward * displacement);
	}

	public void LateUpdate()
	{
		if (IsLive)
		{
			UpdateLineRenderer();
		}
	}

	public void UpdateLineRenderer()
	{
		var headPosition = transform.position;
		var tailPosition = headPosition - transform.forward * MissileLength;

		var tailDotProd = Vector3.Dot(tailPosition - _shootFrom, transform.forward);
		var headHitDotProd = Vector3.Dot(headPosition - _hitPosition, transform.forward);

		if (_hasHit && headHitDotProd > 0f)
		{
			headPosition = _hitPosition;
			var tailHitDotProd = Vector3.Dot(tailPosition - _hitPosition, transform.forward);
			if (tailHitDotProd > 0f)
			{
				Stop();
				return;
			}
		}

		if (tailDotProd < 0f)
		{
			tailPosition = _shootFrom;
		}

		Tracer.SetPosition(0, headPosition);
		Tracer.SetPosition(1, tailPosition);
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
