using UnityEngine;

[RequireComponent(typeof (Shiftable))]
public class SeekingRocket : Missile
{
    public GameObject ExplodePrefab;
    public MeshRenderer Rocket;
    public TrailRenderer Tracer;

    private Shiftable _shiftable;

    private Transform _target;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private Vector3 offsetVelocity;

    private bool isVehicleTarget;
    private Vehicle vehicle;

    private Vector3 velocity;

    private float minChaseDistance = 30f;
    private float explodeDistance = 2f;

    private float travelStraightTime = 1f;
    private float travelStraightCooldown;
    private float noTargetTime = 5f;
    private float noTargetCooldown;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        //_shiftable.OnShift += Shift;
    }

    public override void LiveUpdate()
    {
        if (_target != null)
        {
            if (travelStraightCooldown >= 0)
            {
                travelStraightCooldown -= Time.deltaTime;
            }

            var toTarget = _target.position - transform.position;
            if (travelStraightCooldown < 0f)
            {
                if (isVehicleTarget)
                {
                    if (toTarget.sqrMagnitude > minChaseDistance*minChaseDistance)
                    {
                        offsetVelocity = _initVelocity;
                    }
                    else
                    {
                        offsetVelocity = Vector3.Lerp(offsetVelocity, vehicle.GetVelocity(), 5f*Time.deltaTime);

                    }
                }
                else
                {
                    if (toTarget.sqrMagnitude > minChaseDistance*minChaseDistance)
                    {
                        offsetVelocity = _initVelocity;
                    }
                    else
                    {
                        offsetVelocity = Vector3.Lerp(offsetVelocity, Vector3.zero, 5f*Time.deltaTime);
                    }
                }

                if (toTarget.sqrMagnitude > minChaseDistance*minChaseDistance)
                {
                    transform.forward = Vector3.Lerp(transform.forward, toTarget.normalized, 5f*Time.deltaTime);
                }
                else
                {
                    transform.forward = toTarget.normalized;
                }
            }

            if (toTarget.sqrMagnitude < explodeDistance*explodeDistance)
            {
                Explode();
            }
            noTargetCooldown = noTargetTime;
        }

        if (noTargetCooldown >= 0f)
        {
            noTargetCooldown -= Time.deltaTime;
            if (noTargetCooldown < 0f)
            {
                Explode();
            }
        }

        velocity = offsetVelocity + transform.forward*MissileSpeed;

        var displacement = velocity * Time.deltaTime;
        _shiftable.Translate(displacement);
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        _target = target;

        vehicle = _target.GetComponent<Vehicle>();
        isVehicleTarget = vehicle != null;
    }

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        _shootFrom = shootFrom;
        _initVelocity = initVelocity;

        velocity = _initVelocity;

        transform.position = _shootFrom;
        transform.forward = direction;
        transform.position += initVelocity*Time.deltaTime;

        Rocket.enabled = true;
        Tracer.enabled = true;

        noTargetCooldown = noTargetTime;
        travelStraightCooldown = travelStraightTime;
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

        var damageColliders = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Detectable"));
        foreach (var damageCollider in damageColliders)
        {
            var detectable = damageCollider.GetComponent<Detectable>();
            if (detectable != null)
            {
                var killable = detectable.TargetTransform.GetComponent<Killable>();
                killable.Damage(100f, transform.position, Vector3.up, Owner);
            }
        }
        Stop();
    }

    public override void Stop()
    {
        base.Stop();
        Rocket.enabled = false;
        Tracer.enabled = false;
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
    }
}
