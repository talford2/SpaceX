using UnityEngine;

public abstract class Missile : MonoBehaviour
{
	protected bool IsLive;
    protected GameObject Owner;
    protected float Damage;

    [Header("Missile")]
    public float MissileSpeed = 150f;
    public Transform FromReference;

    public GameObject HitEffectPrefab;
    public GameObject HitDecalPrefab;

    public virtual void Initialize(GameObject owner, float damage)
    {
        Owner = owner;
        Damage = damage;
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

    public void PlaceHitEffects(Vector3 position, Vector3 normal, Transform parent)
    {
        // TODO: Should pull this effect from a pool or something...
        if (HitEffectPrefab != null)
        {
            var hitEffectInstance = ResourcePoolManager.GetAvailable(HitEffectPrefab, position, transform.rotation);

            var hitEffectShiftable = hitEffectInstance.GetComponent<Shiftable>();
            if (hitEffectShiftable != null)
            {
                var univPos = Universe.Current.GetUniversePosition(position);
                hitEffectShiftable.SetShiftPosition(univPos);
            }

            hitEffectInstance.transform.position = position;
            hitEffectInstance.transform.forward = normal;
        }

        if (HitDecalPrefab != null)
        {
            var hitDecal = Instantiate(HitDecalPrefab);
            hitDecal.transform.position = position;
            hitDecal.transform.SetParent(parent);
            hitDecal.transform.forward = normal;
        }
    }

    public virtual void SetTarget(Transform target)
    {
    }
}
