﻿using UnityEngine;

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

    public delegate void OnLostShieldEvent(Killable sender, GameObject attacker);
    public event OnLostShieldEvent OnLostShield;

	public delegate void OnDieEvent(Killable sender, GameObject attacker);
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
                {
                    if (OnLostShield != null)
                        OnLostShield(this, attacker);
                    Shield = 0f;
                }
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
                Die(attacker);
        }
    }

	public void Die(GameObject attacker)
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
			OnDie(this, attacker);

        if (attacker != null && PlayerController.Current != null)
        {
            Debug.LogFormat("{0} KILLED BY {1}", gameObject, attacker);
            if (attacker == PlayerController.Current.VehicleInstance.gameObject)
            {
                PlayerController.Current.OnPlayerKill(this);
            }
        }

		if (DestroyOnDie)
		{
			Destroy(gameObject);
		}
	}
}
