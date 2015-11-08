using UnityEngine;

public class Missile : MonoBehaviour
{
    public float MissileLength = 6f;
    public float MissileSpeed = 150f;
    public float MissileDamage = 5f;

    private bool _isLive;
    private LineRenderer _lineRenderer;
    private Shiftable _shiftable;
    private bool _hasHit;

    private Vector3 _shootFrom;
    private Vector3 _hitPosition;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Shift;
        Stop();
    }

    public void Update()
    {
        if (_isLive)
        {
            var displacement = MissileSpeed*Time.deltaTime;
            if (!_hasHit)
            {
                var missileRay = new Ray(transform.position, transform.forward);
                RaycastHit missileHit;
                if (Physics.Raycast(missileRay, out missileHit, displacement))
                {
                    var killable = missileHit.collider.GetComponentInParent<Killable>();
                    if (killable != null)
                    {
                        killable.Damage(MissileDamage);
                        _hasHit = true;
                        _hitPosition = missileHit.point;
                    }
                }
            }
            transform.position += transform.forward*displacement;
        }
    }

    public void LateUpdate()
    {
        if (_isLive)
        {
            UpdateLineRenderer();
        }
    }

    public void UpdateLineRenderer()
    {
        var headPosition = transform.position;
        var tailPosition = transform.position - transform.forward*MissileLength;

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

        _lineRenderer.SetPosition(0, headPosition);
        _lineRenderer.SetPosition(1, tailPosition);
    }

    public void Stop()
    {
        _isLive = false;
        _lineRenderer.enabled = false;
        _hasHit = false;
    }

    public void Shoot(Vector3 shootFrom)
    {
        _isLive = true;
        _lineRenderer.enabled = true;
        _shootFrom = shootFrom;
        _hasHit = false;
        UpdateLineRenderer();
    }

    private void Shift(Vector3 delta)
    {
        _shootFrom -= delta;
        _hitPosition -= delta;
    }
}
