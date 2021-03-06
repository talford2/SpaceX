﻿using UnityEngine;

public abstract class Missile : MonoBehaviour
{
    protected bool IsLive;
    protected GameObject Owner;
    protected float Damage;

    [Header("Missile")]
    public float MissileSpeed = 150f;
    public float MissileForce = 50f;
    public Transform FromReference;

    public GameObject HitEffectPrefab;
    public GameObject HitDecalPrefab;

    public virtual void Initialize(GameObject owner, float damage)
    {
        Owner = owner;
        Damage = damage;
        Stop();
    }

    private void Start()
    {
        Stop();
    }

    public void Update()
    {
        if (IsLive)
        {
            LiveUpdate();
        }
    }

    public abstract void LiveUpdate();

    public virtual void Stop()
    {
        IsLive = false;
    }

    public virtual void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
    {
        IsLive = true;
    }

    private GameObject _hitEffectInstance;
    public void PlaceHitEffects(Vector3 position, Vector3 normal, Transform parent)
    {
        // TODO: Should pull this effect from a pool or something...
        if (HitEffectPrefab != null)
        {
            _hitEffectInstance = ResourcePoolManager.GetAvailable(HitEffectPrefab, position, transform.rotation);

            var hitEffectShiftable = _hitEffectInstance.GetComponent<Shiftable>();
            if (hitEffectShiftable != null)
            {
                hitEffectShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(position));
            }

            _hitEffectInstance.transform.position = position;
            _hitEffectInstance.transform.forward = normal;
        }

        //if (HitDecalPrefab != null)
        //{
        //    var hitDecal = Instantiate(HitDecalPrefab);
        //    hitDecal.transform.position = position;
        //    hitDecal.transform.SetParent(parent);
        //    hitDecal.transform.forward = normal;
        //}
    }

    public virtual void SetTarget(Transform target)
    {
    }
}
