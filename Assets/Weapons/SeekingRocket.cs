using UnityEngine;

[RequireComponent(typeof (Shiftable))]
public class SeekingRocket : Missile
{
    private Shiftable _shiftable;

    private Transform _target;
    private Vector3 _shootFrom;
    private Vector3 _initVelocity;
    private float _initSpeed;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Shift;
    }

    public override void LiveUpdate()
    {
        if (_target != null)
        {
            var toTarget = _target.position - transform.position;
            if (toTarget.sqrMagnitude > 400f)
            {
                transform.forward = toTarget.normalized;
            }
        }
        var displacement = (_initSpeed + MissileSpeed)*Time.deltaTime;
        _shiftable.Translate(transform.forward*displacement);
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
        _initSpeed = _initVelocity.magnitude;
        transform.position = _shootFrom;
        transform.forward = direction;
        transform.position += initVelocity*Time.deltaTime;
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
    }
}
