using UnityEngine;

public class Killable : MonoBehaviour
{
    public float Health;
    public float MaxHealth;

    public GameObject DamageEffect;
    public GameObject DieEffect;

    public delegate void OnDamageEvent(Vector3 position, Vector3 normal);
    public event OnDamageEvent OnDamage;

    public delegate void OnDieEvent();
    public event OnDieEvent OnDie;

    private bool isAlive;

    private void Awake()
    {
        isAlive = true;
    }

    public void Damage(float damage, Vector3 position, Vector3 normal)
    {
        if (isAlive)
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
        isAlive = false;
        if (OnDie != null)
            OnDie();
        if (DieEffect != null)
            Instantiate(DieEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
