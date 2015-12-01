using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Killable))]
public class HealthRegenerator : MonoBehaviour
{
    public float RegenerationRate;
    public float RegenerationDelay;

    private Killable killable;
    private float delayCooldown;

    private void Awake()
    {
        killable = GetComponent<Killable>();
        killable.OnDamage += HealthRegenerator_OnDamage;
    }

    private void Update()
    {
        if (delayCooldown >= 0f)
        {
            delayCooldown -= Time.deltaTime;
        }
        if (delayCooldown < 0f)
        {
            if (killable.Health < killable.MaxHealth)
            {
                killable.Health += RegenerationRate*Time.deltaTime;
                if (killable.Health > killable.MaxHealth)
                    killable.Health = killable.MaxHealth;
            }
        }
    }

    private void HealthRegenerator_OnDamage(Vector3 position, Vector3 normal)
    {
        delayCooldown = RegenerationDelay;
    }
}
