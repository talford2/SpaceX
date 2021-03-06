﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Killable))]
public class VehicleCorpse : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public GameObject DebrisPrefab;
    public List<ParticleSystem> SmokeSystems;

    public float CollisionRadius = 3f;

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
    private float _rotateDir;

    private bool _isExplosive;
    private Quaternion _initRotVelocity;

    private void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        _shiftable = GetComponent<Shiftable>();
        _killable = GetComponent<Killable>();

        _killable.OnDie += CorpseExplode;
        _detectableMask = LayerMask.GetMask("Detectable");
        _collisionMask = LayerMask.GetMask("Environment", "Default", "Player");
    }

    private void Start()
    {
        StartCoroutine(DelayedExplode(Random.Range(2f, 3f)));
    }

    public void Initialize(Vector3 initialVelocity, Vector3 hitPosition, bool explosive)
    {
        _initialVelocity = initialVelocity;
        _rotationSpeed = 0f;
        _rotateDir = Utility.RandomSign();

        _isExplosive = explosive;
        if (explosive)
        {
            var delta = hitPosition - transform.position;
            _initialVelocity = initialVelocity - 20f * delta.normalized;
            _initRotVelocity = Quaternion.Euler(delta);
        }
    }

    private void Update()
    {
        if (_isExplosive)
        {
            transform.Rotate(_initRotVelocity.eulerAngles);
        }
        else
        {
            _rotationSpeed = Mathf.MoveTowards(_rotationSpeed, _maxRotationSpeed, _rotationAcceleration * Time.deltaTime);
            transform.Rotate(Vector3.forward, _rotateDir * _rotationSpeed * Time.deltaTime);
        }
        if (Physics.SphereCast(new Ray(transform.position, _initialVelocity.normalized), CollisionRadius, (_initialVelocity.magnitude - CollisionRadius) + 0.01f, _collisionMask))
            CorpseExplode(null, Vector3.zero, Vector3.up, null);
        _shiftable.Translate(_initialVelocity * Time.deltaTime);
    }

    private IEnumerator DelayedExplode(float delay)
    {
        yield return new WaitForSeconds(delay);
        CorpseExplode(null, Vector3.zero, Vector3.up, null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _killable.Damage(0.01f * collision.impulse.magnitude, collision.contacts[0].point, collision.contacts[0].normal, null);
    }

    private void CorpseExplode(Killable sender, Vector3 positon, Vector3 normal, GameObject attacker)
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
