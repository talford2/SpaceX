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

    private Vector3 velocity;

    private float minChaseDistance = 20f;
    private float explodeDistance = 2f;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        //_shiftable.OnShift += Shift;
    }

    public override void LiveUpdate()
    {
        if (_target != null)
        {
            var toTarget = _target.position - transform.position;
            if (toTarget.sqrMagnitude > minChaseDistance*minChaseDistance)
            {
                transform.forward = toTarget.normalized;
                velocity = _initVelocity + transform.forward*MissileSpeed;
            }
            else
            {
                var vehicle = _target.GetComponent<Vehicle>();
                if (vehicle != null)
                {
                    transform.forward = toTarget.normalized;
                    velocity = Vector3.Lerp(velocity, vehicle.GetVelocity() + transform.forward * MissileSpeed, 5f*Time.deltaTime);
                }
            }

            if (toTarget.sqrMagnitude < explodeDistance*explodeDistance)
            {
                Explode();
            }
        }
        var displacement = velocity * Time.deltaTime;
        _shiftable.Translate(displacement);
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        _target = target;
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
