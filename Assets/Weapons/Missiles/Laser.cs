using UnityEngine;

public class Laser : Missile {
    public float MissileLength = 6f;

    private LineRenderer _lineRenderer;
    private Shiftable _shiftable;
    private bool _hasHit;

    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private float _initSpeed;
    private Vector3 _hitPosition;

    private Vector3 observationPosition;
    private float rayCheckMaxDistSquared = 25000000f;
    private float stopDistanceSquared = 100000000f;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Shift;
    }

    public override void LiveUpdate()
    {
        var displacement = (_initSpeed + MissileSpeed) * Time.deltaTime;
        observationPosition = Universe.Current.ViewPort.transform.position;
        var toOberverSquared = (transform.position - observationPosition).sqrMagnitude;
        if (toOberverSquared < rayCheckMaxDistSquared)
        {
            if (!_hasHit)
            {
                var missileRay = new Ray(transform.position, transform.forward);
                RaycastHit missileHit;
                if (Physics.Raycast(missileRay, out missileHit, displacement, ~LayerMask.GetMask("Distant", "Universe Background")))
                {
                    if (missileHit.collider.gameObject != Owner)
                    {
                        var killable = missileHit.collider.GetComponentInParent<Killable>();
                        if (killable != null)
                        {
                            killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
                            _hasHit = true;
                            _hitPosition = missileHit.point;

                            //killable.SendMessageUpwards("Hit", new HitEffectParameters { Position = missileHit.point, Normal = missileHit.normal });
                            PlaceHitEffects(missileHit.point, missileHit.normal, missileHit.collider.gameObject.transform);
                        }
                    }
                }
            }
        }
        else
        {
            if (toOberverSquared > stopDistanceSquared)
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
        else
        {
            if (Owner == null)
                Destroy(gameObject);
        }
    }

    public void UpdateLineRenderer()
    {
        var headPosition = transform.position;
        var tailPosition = transform.position - transform.forward * MissileLength;

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

        _lineRenderer.SetPosition(0, tailPosition);
        _lineRenderer.SetPosition(1, headPosition);
    }

    public override void Stop()
    {
        base.Stop();
        _lineRenderer.enabled = false;
        _hasHit = false;
        if (Owner == null)
            Destroy(gameObject);
    }

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        _lineRenderer.enabled = true;
        _shootFrom = shootFrom;
        _initVelocity = initVelocity;
        _initSpeed = _initVelocity.magnitude;
        _hasHit = false;
        transform.position = _shootFrom;
        transform.position += initVelocity * Time.deltaTime;
        transform.forward = direction;
        UpdateLineRenderer();
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
        _shootFrom -= delta;
        _hitPosition -= delta;
        UpdateLineRenderer();
    }
}
