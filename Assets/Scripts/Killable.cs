using UnityEngine;

public class Killable : MonoBehaviour
{
	public float Health;
	public float MaxHealth;

    public float Shield;
    public float MaxShield;

	public GameObject DamageEffect;
	public GameObject DieEffect;

	public delegate void OnDamageEvent(Killable sender, Vector3 position, Vector3 normal, GameObject attacker);
	public event OnDamageEvent OnDamage;

	public delegate void OnDieEvent(Killable sender);
	public event OnDieEvent OnDie;

	public bool DestroyOnDie = true;

	public bool IsAlive { get; set; }

    private void Awake()
    {
        IsAlive = true;
    }

    public void Damage(float damage, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (IsAlive)
        {
            if (Shield > 0f)
            {
                Shield -= damage;
                if (Shield < 0f)
                    Shield = 0f;
            }
            else
            {
                Health -= damage;
            }
            if (OnDamage != null)
                OnDamage(this, position, normal, attacker);
            if (DamageEffect != null)
                Instantiate(DamageEffect, position, Quaternion.LookRotation(normal));
            if (Health <= 0f)
                Die();
        }
    }

	public void Die()
	{
		if (DieEffect != null)
		{
            var dieInst = ResourcePoolManager.GetAvailable(DieEffect, transform.position, transform.rotation);

			dieInst.transform.localScale = transform.localScale;
			var dieShiftable = dieInst.GetComponent<Shiftable>();

			if (dieShiftable != null)
				dieShiftable.SetShiftPosition(Universe.Current.GetUniversePosition(transform.position));
		}

		IsAlive = false;
		if (OnDie != null)
			OnDie(this);

		if (DestroyOnDie)
		{
			Destroy(gameObject);
		}
	}
}
