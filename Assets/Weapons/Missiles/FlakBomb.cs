using UnityEngine;
using System.Collections;

public class FlakBomb : Missile
{
    [Header("Flak Bomb")]
    public float MinChaseDistance = 50f;
    public float MaxTurnSpeed = 90f;
    public float Radius = 0.3f;

    public float LifeTimeMin = 5f;
    public float LifeTimeMax = 5f;

    public GameObject ExplodePrefab;
    public MeshRenderer Rocket;
    public float MinExplodeRadius = 8f;
    public float MaxExplodeRadius = 30f;
    public float MaxExplodeDamage = 120f;
    public Trail Tracer;

    private Shiftable _shiftable;
    private ResourcePoolItem _resourcePoolItem;

    private Transform _target;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;

    private bool _isVehicleTarget;
    private Vehicle _vehicle;

    private Vector3 _velocity;

    private float _explodeDistance = 2f;

    private float _travelStraightTime = 0.5f;
    private float _travelStraightCooldown;
    private float _noTargetCooldown;
    private float _delayTargetTime = 0.5f;
    private float _delayTargetCooldown;

    private float _turnTime = 2f;

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

    private Collider[] _targetColliders = new Collider[10];

    public override void LiveUpdate()
    {
        if (_target != null)
        {
            var toTarget = _target.position - transform.position;
            if (Vector3.Dot(toTarget.normalized, transform.forward) < 0)
            {
                _noTargetCooldown = Random.Range(0f, 0.1f);
            }
        }

        if (_noTargetCooldown >= 0f)
        {
            _noTargetCooldown -= Time.deltaTime;
            if (_noTargetCooldown < 0f)
            {
                Explode();
            }
        }

        _velocity = _initVelocity + transform.forward * MissileSpeed;

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

        if (_delayTargetCooldown >= 0f)
            _delayTargetCooldown -= Time.deltaTime;

        if (!_hasHit)
        {
            if (_delayTargetCooldown < 0f)
            {
                var count = Physics.OverlapSphereNonAlloc(transform.position, MinExplodeRadius, _damageColliders, _detectableMask);
                bool isExplode = false;
                for (var i = 0; i < count; i++)
                {
                    var killable = _damageColliders[i].GetComponentInParent<Killable>();
                    if (killable != null)
                    {
                        isExplode = true;
                    }
                }
                if (isExplode)
                {
                    Explode();
                    return;
                }
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
        _initVelocity = initVelocity;

        _velocity = _initVelocity;

        transform.position = _shootFrom;
        transform.forward = direction;// FromReference.forward;
        transform.position += initVelocity * Time.deltaTime;

        Rocket.enabled = true;

        Tracer.Reset();

        _noTargetCooldown = Random.Range(LifeTimeMin, LifeTimeMax);
        _delayTargetCooldown = _delayTargetTime;
        _travelStraightCooldown = _travelStraightTime + _turnTime;
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

        if (explodeShiftable != null && ownerShiftable != null)
        {
            var univPos = Universe.Current.GetUniversePosition(transform.position);
            explodeShiftable.SetShiftPosition(univPos);
        }

        var shakeSource = explodeInstance.GetComponent<ScreenShakeSource>();
        if (shakeSource != null)
            shakeSource.Trigger();

        SplashDamage.ExplodeAt(transform.position, MaxExplodeRadius, MinExplodeRadius, MaxExplodeDamage, MissileForce, _detectableMask, Owner);

        Stop();
    }

    

    public override void Stop()
    {
        base.Stop();
        Rocket.enabled = false;
        _target = null;
        if (_resourcePoolItem != null)
            _resourcePoolItem.IsAvailable = true;
    }
}
