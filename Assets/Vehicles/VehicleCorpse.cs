using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Killable))]
public class VehicleCorpse : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public GameObject DebrisPrefab;
    public GameObject SmokeInstance;

    [Header("Explosion")]
    public float MaxExplodeRadius = 30f;
    public float MinExplodeRadius = 8f;
    public float MaxExplodeDamage = 100f;
    public float ExplodeForce = 100f;

    private Rigidbody _rBody;
    private Killable _killable;
    private int _detectableMask;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
        _killable = GetComponent<Killable>();

        _killable.OnDie += CorpseExplode;
        _detectableMask = LayerMask.GetMask("Detectable");
    }

    private void Start()
    {
        StartCoroutine(DelayedExplode(Random.Range(2f, 3f)));
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
            var explodeInstance = ResourcePoolManager.GetAvailable(ExplosionPrefab, _rBody.position, _rBody.rotation);
            explodeInstance.transform.localScale = transform.localScale;
            var explodeShiftable = explodeInstance.GetComponent<Shiftable>();

            if (explodeShiftable != null)
                explodeShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(_rBody.position));
        }

        if (DebrisPrefab != null)
        {
            var debrisInstance = (GameObject)Instantiate(DebrisPrefab, _rBody.position, _rBody.rotation);
            var rBodies = debrisInstance.GetComponentsInChildren<Rigidbody>();
            foreach (var debrisrBody in rBodies)
            {
                debrisrBody.velocity = _rBody.velocity;
                debrisrBody.AddExplosionForce(200f, transform.position, 20f, 0f, ForceMode.Impulse);
                var debrisShiftable = debrisrBody.GetComponent<Shiftable>();
                debrisShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(debrisrBody.position));
            }
        }

        if (SmokeInstance != null)
        {
            SmokeInstance.transform.parent = null;
            var woundParticles = SmokeInstance.GetComponent<ParticleSystem>();
            if (woundParticles != null)
                woundParticles.Stop();
            StartCoroutine(DelayedSmokeDestroy(woundParticles.duration + 0.5f));
        }

        SplashDamage.ExplodeAt(transform.position, MaxExplodeRadius, MinExplodeRadius, MaxExplodeDamage, ExplodeForce, _detectableMask, gameObject);

        Destroy(gameObject);
    }

    private IEnumerator DelayedSmokeDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(SmokeInstance);
    }
}
