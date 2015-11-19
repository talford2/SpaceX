using UnityEngine;

public class Killable : MonoBehaviour
{
	public float Health;
	public float MaxHealth;

	public GameObject DamageEffect;
	public GameObject DieEffect;

	public delegate void OnDamageEvent(Vector3 position, Vector3 normal);
	public event OnDamageEvent OnDamage;

	public delegate void OnDieEvent(Killable sender);
	public event OnDieEvent OnDie;

	public bool DestroyOnDie = true;

	public bool IsAlive { get; set; }

	private void Awake()
	{
		IsAlive = true;
	}

	public void Damage(float damage, Vector3 position, Vector3 normal)
	{
		if (IsAlive)
		{
			Health -= damage;
			if (OnDamage != null)
				OnDamage(position, normal);
			if (DamageEffect != null)
				Instantiate(DamageEffect, position, Quaternion.LookRotation(normal));
			if (Health < 0f)
				Die();
		}
	}

	public void Die()
	{
		if (DieEffect != null)
		{
			Utility.InstantiateInParent(DieEffect, transform.position, transform.rotation, transform.parent);
			//Instantiate(DieEffect, transform.position, transform.rotation);
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
