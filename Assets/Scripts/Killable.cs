using UnityEngine;

public class Killable : MonoBehaviour
{
    public float Health;
    public float MaxHealth;

    public delegate void OnDamageEvent();
    public event OnDamageEvent OnDamage;

    public delegate void OnDieEvent();
    public event OnDieEvent OnDie;

    private bool isAlive;

    private void Awake()
    {
        isAlive = true;
    }

    public void Damage(float damage)
    {
        if (isAlive)
        {
            Health -= damage;
            if (OnDamage != null)
                OnDamage();
            if (Health < 0f)
                Die();
        }
    }

    public void Die()
    {
        isAlive = false;
        if (OnDie != null)
            OnDie();
    }
}
