using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class SeekingRocket : Missile
{
    [Header("Seeking Rocket")]
    public float MinChaseDistance = 50f;
    public float StraightTravelTime = 0.5f;
    public float MaxTurnTime = 2f;
    public float MaxTurnSpeed = 90f;
    public float Radius = 0.3f;
    public GameObject ExplodePrefab;
    public MeshRenderer Rocket;
    public MeshRenderer ThrusterMesh;
    public Trail Tracer;
    public FlareDistanceBrightness Flare;

    private Shiftable _shiftable;
    private ResourcePoolItem _resourcePoolItem;

    private Transform _target;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private Vector3 _offsetVelocity;

    private bool _isVehicleTarget;
    private Vehicle _vehicle;

    private Vector3 _velocity;

    private float _explodeDistance = 2f;

    private float _travelStraightCooldown;
    private float _noTargetTime = 5f;
    private float _noTargetCooldown;

    // Spherecast hit
    private bool _hasHit;
    private Vector3 _hitPosition;
    private Vector3 _observationPosition;
    private float _rayCheckMaxDistSquared = 25000000f;
    private float _stopDistanceSquared = 100000000f;

    private int _mask;

    private int _detectableMask;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _mask = ~LayerMask.GetMask("Distant", "Universe Background", "Environment", "Detectable");
        _detectableMask = LayerMask.GetMask("Detectable");
    }

    public override void Initialize(GameObject owner, float damage)
    {
        if (_resourcePoolItem == null)
            _resourcePoolItem = GetComponent<ResourcePoolItem>();
        base.Initialize(owner, damage);
    }

    public override void LiveUpdate()
    {
        if (_target != null)
        {
            if (_travelStraightCooldown >= 0)
            {
                _travelStraightCooldown -= Time.deltaTime;
            }

            var toTarget = _target.position - transform.position;
            if (_isVehicleTarget)
            {
                if (toTarget.sqrMagnitude > MinChaseDistance * MinChaseDistance)
                {
                    _offsetVelocity = _initVelocity;
                }
                else
                {
                    _offsetVelocity = Vector3.Lerp(_offsetVelocity, _vehicle.GetVelocity(), 5f * Time.deltaTime);
                }
            }
            else
            {
                if (toTarget.sqrMagnitude > MinChaseDistance * MinChaseDistance)
                {
                    _offsetVelocity = _initVelocity;
                }
                else
                {
                    _offsetVelocity = Vector3.Lerp(_offsetVelocity, Vector3.zero, 5f * Time.deltaTime);
                }
            }

            if (_travelStraightCooldown < MaxTurnTime)
            {
                var turnFraction = Mathf.Clamp01(1f - _travelStraightCooldown / MaxTurnTime);
                var maxTurn = MaxTurnSpeed * turnFraction;
                transform.forward = Vector3.RotateTowards(transform.forward, toTarget.normalized, maxTurn * Time.deltaTime, 0f);
            }

            if (toTarget.sqrMagnitude < _explodeDistance * _explodeDistance)
            {
                Explode();
            }
            _noTargetCooldown = _noTargetTime;
        }

        if (_noTargetCooldown >= 0f)
        {
            _noTargetCooldown -= Time.deltaTime;
            if (_noTargetCooldown < 0f)
            {
                Explode();
            }
        }

        _velocity = _offsetVelocity + transform.forward * MissileSpeed;

        var displacement = _velocity * Time.deltaTime;

        // Hit objects
        if (_hasHit)
        {
            Explode();
            return;
        }
        _observationPosition = Universe.Current.ViewPort.transform.position;
        var toOberverSquared = (transform.position - _observationPosition).sqrMagnitude;
        if (toOberverSquared < _rayCheckMaxDistSquared)
        {
            if (!_hasHit)
            {
                var missileRay = new Ray(transform.position, _velocity);
                RaycastHit missileHit;
                if (Physics.SphereCast(missileRay, Radius, out missileHit, displacement.magnitude, _mask))
                {
                    if (missileHit.collider.gameObject != Owner)
                    {
                        var killable = missileHit.collider.GetComponentInParent<Killable>();
                        if (killable != null)
                        {
                            killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
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
            if (toOberverSquared > _stopDistanceSquared)
            {
                Stop();
            }
        }

        _shiftable.Translate(displacement);
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        _target = target;

        _vehicle = _target.GetComponent<Vehicle>();
        _isVehicleTarget = _vehicle != null;
        _resourcePoolItem.IsAvailable = false;
    }

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        _shootFrom = shootFrom;
        _initVelocity = initVelocity;// Vector3.Dot(initVelocity.normalized, direction.normalized) * initVelocity.magnitude * FromReference.forward;

        _velocity = _initVelocity;

        transform.position = _shootFrom;
        transform.forward = direction;// FromReference.forward;
        transform.position += initVelocity * Time.deltaTime;

        Rocket.enabled = true;
        ThrusterMesh.enabled = true;

        Tracer.Reset();
        //Tracer.Initialize(transform.position);
        Flare.SetVisible(true);

        _noTargetCooldown = _noTargetTime;
        _travelStraightCooldown = StraightTravelTime + MaxTurnTime;
        _hasHit = false;

        _resourcePoolItem.IsAvailable = false;
    }

    private Collider[] _damageColliders = new Collider[10];
    private Detectable _detectable;
    private Killable _killable;
    private void Explode()
    {
        var explodeInstance = ResourcePoolManager.GetAvailable(ExplodePrefab, transform.position, transform.rotation); //(GameObject)Instantiate(ExplodePrefab, transform.position, transform.rotation);

        explodeInstance.transform.localScale = transform.localScale;
        var explodeShiftable = explodeInstance.GetComponent<Shiftable>();
        var ownerShiftable = GetComponent<Shiftable>();

        if (_target != null)
        {
            var targetable = _target.GetComponent<Targetable>();
            if (targetable != null)
            {
                targetable.LockedOnBy = null;
                Debug.Log("TURN LOCK OFF!");
            }
        }

        if (explodeShiftable != null && ownerShiftable != null)
        {
            var univPos = Universe.Current.GetUniversePosition(transform.position);
            explodeShiftable.SetShiftPosition(univPos);
        }

        SplashDamage.ExplodeAt(transform.position, 15f, 5f, 100f, MissileForce, _detectableMask, Owner);

        Stop();
    }

    public override void Stop()
    {
        base.Stop();
        _target = null;
        Rocket.enabled = false;
        ThrusterMesh.enabled = false;
        //Tracer.Stop();
        Flare.SetVisible(false);
        if (_resourcePoolItem != null)
            _resourcePoolItem.IsAvailable = true;
    }
}
