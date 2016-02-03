using UnityEngine;

public abstract class Missile : MonoBehaviour
{
	protected bool IsLive;
    protected GameObject Owner;
    protected float Damage;

    public float MissileSpeed = 150f;

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

    public virtual void SetTarget(Transform target)
    {
    }
}
