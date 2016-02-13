using UnityEngine;

public class Killable : MonoBehaviour
{
	public float Health;
	public float MaxHealth;

	public GameObject DamageEffect;
	public GameObject DieEffect;

	public GameObject WoundEffect;
	private GameObject _woundObj;

	public delegate void OnDamageEvent(Vector3 position, Vector3 normal, GameObject attacker);
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
            Health -= damage;
            if (OnDamage != null)
                OnDamage(position, normal, attacker);
            if (DamageEffect != null)
                Instantiate(DamageEffect, position, Quaternion.LookRotation(normal));
            if (Health <= 0f)
                Die();

            if (Health/MaxHealth < 0.5f)
            {
                StartWoundEffect();
            }
        }
    }

    private void StartWoundEffect()
    {
        if (_woundObj == null && WoundEffect != null)
        {
            _woundObj = Instantiate(WoundEffect);
            _woundObj.transform.parent = transform;
            _woundObj.transform.localPosition = Vector3.zero;
            GetComponent<Shiftable>().ShiftParticleSystems.Add(_woundObj.GetComponent<ParticleSystem>());
        }
    }

    public void StopWoundEffect()
    {
        if (_woundObj != null)
        {
            Destroy(_woundObj);
        }
    }

	public void Die()
	{
		if (DieEffect != null)
		{
			var dieInst = Utility.InstantiateInParent(DieEffect, transform.position, transform.rotation, transform.parent);

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
