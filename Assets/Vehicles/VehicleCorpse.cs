using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Killable))]
public class VehicleCorpse : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public GameObject DebrisPrefab;
    public List<ParticleSystem> SmokeSystems;

    public float CollisionRadius;

    [Header("Explosion")]
    public float MaxExplodeRadius = 30f;
    public float MinExplodeRadius = 8f;
    public float MaxExplodeDamage = 100f;
    public float ExplodeForce = 100f;

    private Shiftable _shiftable;
    private Vector3 _initialVelocity;
    private Killable _killable;
    private int _detectableMask;
    private int _collisionMask;

    private float _rotationAcceleration = 600f;
    private float _maxRotationSpeed = 450f;
    private float _rotationSpeed;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        _shiftable = GetComponent<Shiftable>();
        _killable = GetComponent<Killable>();

        _killable.OnDie += CorpseExplode;
        _detectableMask = LayerMask.GetMask("Detectable");
        _collisionMask = LayerMask.GetMask("Environment", "Default");
    }

    private void Start()
    {
        StartCoroutine(DelayedExplode(Random.Range(2f, 3f)));
    }

    public void Initialize(Vector3 initialVelocity)
    {
        _initialVelocity = initialVelocity;
        _rotationSpeed = 0f;
    }

    private void Update()
    {
        _rotationSpeed = Mathf.MoveTowards(_rotationSpeed, _maxRotationSpeed, _rotationAcceleration * Time.deltaTime);
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        if (Physics.SphereCast(new Ray(transform.position, _initialVelocity.normalized), CollisionRadius, (_initialVelocity.magnitude - CollisionRadius) + 0.01f, _collisionMask))
            CorpseExplode(null, null);
        _shiftable.Translate(_initialVelocity * Time.deltaTime);
    }

    private IEnumerator DelayedExplode(float delay)
    {
        yield return new WaitForSeconds(delay);
        CorpseExplode(null, null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _killable.Damage(0.01f * collision.impulse.magnitude, collision.contacts[0].point, collision.contacts[0].normal, null);
    }

    private void CorpseExplode(Killable sender, GameObject attacker)
    {
        if (ExplosionPrefab != null)
        {
            var explodeInstance = ResourcePoolManager.GetAvailable(ExplosionPrefab, transform.position, transform.rotation);
            explodeInstance.transform.localScale = transform.localScale;
            var explodeShiftable = explodeInstance.GetComponent<Shiftable>();

            if (explodeShiftable != null)
                explodeShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(transform.position));
        }

        if (DebrisPrefab != null)
        {
            var debrisInstance = (GameObject)Instantiate(DebrisPrefab, transform.position, transform.rotation);
            var rBodies = debrisInstance.GetComponentsInChildren<Rigidbody>();
            foreach (var debrisrBody in rBodies)
            {
                debrisrBody.velocity = _initialVelocity;
                debrisrBody.AddExplosionForce(200f, transform.position, 20f, 0f, ForceMode.Impulse);
                var debrisShiftable = debrisrBody.GetComponent<Shiftable>();
                debrisShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(debrisrBody.position));
            }
        }

        if (SmokeSystems != null)
        {
            foreach (var smokeSystem in SmokeSystems)
            {
                smokeSystem.transform.parent = null;
                if (smokeSystem != null)
                    smokeSystem.Stop();
                StartCoroutine(DelayedSmokeDestroy(smokeSystem.duration + 0.5f));
            }
        }

        SplashDamage.ExplodeAt(transform.position, MaxExplodeRadius, MinExplodeRadius, MaxExplodeDamage, ExplodeForce, _detectableMask, gameObject);

        Destroy(gameObject);
    }

    private IEnumerator DelayedSmokeDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var smokeSystem in SmokeSystems)
        {
            Destroy(smokeSystem.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CollisionRadius);
    }
}
