using UnityEngine;

[RequireComponent(typeof (Shiftable))]
public class SeekingRocket : Missile
{
    [Header("Seeking Rocket")]
    public float MinChaseDistance = 50f;
    public float MaxTurnSpeed = 90f;
    public GameObject ExplodePrefab;
    public MeshRenderer Rocket;
    public MeshRenderer ThrusterMesh;
    public ShiftTrail Tracer;
    public FlareDistanceBrightness Flare;

    private Shiftable _shiftable;

    private Transform _target;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private Vector3 _offsetVelocity;

    private bool _isVehicleTarget;
    private Vehicle _vehicle;

    private Vector3 _velocity;

    private float _explodeDistance = 2f;

    private float _travelStraightTime = 0.5f;
    private float _travelStraightCooldown;
    private float _noTargetTime = 5f;
    private float _noTargetCooldown;

    private float _turnTime = 2f;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        //_shiftable.OnShift += Shift;
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
                if (toTarget.sqrMagnitude > MinChaseDistance*MinChaseDistance)
                {
                    _offsetVelocity = _initVelocity;
                }
                else
                {
                    _offsetVelocity = Vector3.Lerp(_offsetVelocity, _vehicle.GetVelocity(), 5f*Time.deltaTime);
                }
            }
            else
            {
                if (toTarget.sqrMagnitude > MinChaseDistance*MinChaseDistance)
                {
                    _offsetVelocity = _initVelocity;
                }
                else
                {
                    _offsetVelocity = Vector3.Lerp(_offsetVelocity, Vector3.zero, 5f*Time.deltaTime);
                }
            }
 
            if (_travelStraightCooldown < _turnTime)
            {
                var turnFraction = Mathf.Clamp01(1f - _travelStraightCooldown/_turnTime);
                var maxTurn = MaxTurnSpeed*turnFraction;
                transform.forward = Vector3.RotateTowards(transform.forward, toTarget.normalized, maxTurn*Time.deltaTime, 0f);
            }

            if (toTarget.sqrMagnitude < _explodeDistance*_explodeDistance)
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

        _velocity = _offsetVelocity + transform.forward*MissileSpeed;

        var displacement = _velocity*Time.deltaTime;
        _shiftable.Translate(displacement);
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        _target = target;

        _vehicle = _target.GetComponent<Vehicle>();
        _isVehicleTarget = _vehicle != null;
    }

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        _shootFrom = shootFrom;
        _initVelocity = initVelocity;

        _velocity = _initVelocity;

        transform.position = _shootFrom;
        transform.forward = direction;
        transform.position += initVelocity*Time.deltaTime;

        Rocket.enabled = true;
        ThrusterMesh.enabled = true;

        Tracer.Initialize(transform.position);
        Flare.SetVisible(true);

        _noTargetCooldown = _noTargetTime;
        _travelStraightCooldown = _travelStraightTime + _turnTime;
    }

    private void Explode()
    {
        var explodeInstance = (GameObject)Instantiate(ExplodePrefab, transform.position, transform.rotation);

        explodeInstance.transform.localScale = transform.localScale;
        var explodeShiftable = explodeInstance.GetComponent<Shiftable>();
        var ownerShiftable = GetComponent<Shiftable>();

        if (explodeShiftable != null && ownerShiftable != null)
        {
            var univPos = Universe.Current.GetUniversePosition(transform.position);
            explodeShiftable.SetShiftPosition(univPos);
        }

        var damageColliders = Physics.OverlapSphere(transform.position, 15f, LayerMask.GetMask("Detectable"));
        foreach (var damageCollider in damageColliders)
        {
            var detectable = damageCollider.GetComponent<Detectable>();
            if (detectable != null)
            {
                var killable = detectable.TargetTransform.GetComponent<Killable>();
                var damage = Mathf.Round(100f*GetDamageFraction(detectable.transform.position, transform.position, 5f, 15f));
                killable.Damage(damage, transform.position, Vector3.up, Owner);
            }
        }
        Stop();
    }

    private float GetDamageFraction(Vector3 targetPosition, Vector3 damagePosition, float minDistance, float maxDistance)
    {
        var toDamage = targetPosition - damagePosition;
        if (toDamage.sqrMagnitude < minDistance*minDistance)
            return 1f;
        if (toDamage.sqrMagnitude > maxDistance*maxDistance)
            return 0f;
        return 1f - Mathf.Clamp((toDamage.magnitude - minDistance)/(maxDistance - minDistance), 0, 1f);
    }

    public override void Stop()
    {
        base.Stop();
        Rocket.enabled = false;
        ThrusterMesh.enabled = false;
        Tracer.Stop();
        Flare.SetVisible(false);
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
    }
}
