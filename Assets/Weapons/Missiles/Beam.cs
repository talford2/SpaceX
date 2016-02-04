using UnityEngine;

[RequireComponent(typeof(Shiftable))]
[RequireComponent(typeof(LineRenderer))]
public class Beam : Missile
{
    [Header("Beam")]
    public float MissileLength = 1000f;
    public float Radius = 0.5f;
    public float FireTime = 0.2f;

    private Shiftable _shiftable;
    private LineRenderer _lineRenderer;

    private Quaternion dirRotate;
    private float length;
    private float fireCooldown;

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _shiftable.OnShift += Shift;
        _lineRenderer = GetComponent<LineRenderer>();
        length = MissileLength;
    }

    public override void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        base.Shoot(shootFrom, direction, initVelocity);
        _lineRenderer.enabled = true;

        fireCooldown = FireTime;
        transform.position = FromReference.position;

        dirRotate = Quaternion.FromToRotation(FromReference.forward, direction.normalized);

        transform.forward = direction;
        UpdateLineRenderer();
    }

    public override void LiveUpdate()
    {
        if (fireCooldown >= 0f)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown < 0f)
                Stop();
        }

        transform.position = FromReference.position;
        transform.forward = dirRotate*FromReference.forward;

        length = MissileLength;

        var shootRay = new Ray(transform.position, transform.forward);
        RaycastHit missileHit;
        if (Physics.SphereCast(shootRay, Radius, out missileHit, MissileLength, ~LayerMask.GetMask("Distant", "Universe Background")))
        {
            var killable = missileHit.collider.GetComponentInParent<Killable>();
            if (killable != null)
            {
                killable.Damage(Damage, missileHit.point, missileHit.normal, Owner);
                length = missileHit.distance;

                PlaceHitEffects(missileHit.point, missileHit.normal, missileHit.collider.gameObject.transform);
            }
        }
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        var cooldownFraction = Mathf.Clamp01(fireCooldown / FireTime);
        var diameter = 2f*Radius*cooldownFraction;
        _lineRenderer.SetWidth(diameter, diameter);
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward* length);
    }

    public void LateUpdate()
    {
        if (!IsLive)
        {
            if (Owner == null)
                Destroy(gameObject);
        }
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
        UpdateLineRenderer();
    }

    public override void Stop()
    {
        base.Stop();
        _lineRenderer.enabled = false;
        if (Owner == null)
            Destroy(gameObject);
    }
}
