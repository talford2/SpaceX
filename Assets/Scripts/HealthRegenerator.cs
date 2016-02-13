﻿using UnityEngine;

[RequireComponent(typeof(Killable))]
public class HealthRegenerator : MonoBehaviour
{
    public float RegenerationRate;
    public float RegenerationDelay;

    private Killable _killable;
    private float _delayCooldown;

    private void Awake()
    {
        _killable = GetComponent<Killable>();
        _killable.OnDamage += HealthRegenerator_OnDamage;
    }

    private void Update()
    {
        if (_delayCooldown >= 0f)
        {
            _delayCooldown -= Time.deltaTime;
        }
        if (_delayCooldown < 0f)
        {
            if (_killable.Health < _killable.MaxHealth)
            {
                _killable.Health += RegenerationRate*Time.deltaTime;
                if (_killable.Health/_killable.MaxHealth > 0.5f)
                    _killable.StopWoundEffect();
                if (_killable.Health > _killable.MaxHealth)
                    _killable.Health = _killable.MaxHealth;
            }
        }
    }

    private void HealthRegenerator_OnDamage(Vector3 position, Vector3 normal, GameObject attacker)
    {
        _delayCooldown = RegenerationDelay;
    }
}
