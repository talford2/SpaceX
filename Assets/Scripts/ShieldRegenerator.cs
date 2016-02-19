using UnityEngine;

[RequireComponent(typeof (Killable))]
public class ShieldRegenerator : MonoBehaviour
{
    public float RegenerationRate;
    public float RegenerationDelay;
    public OnRegeneratorRegenerate OnRegenerate;

    public delegate void OnRegeneratorRegenerate();

    private Killable _killable;
    private float _delayCooldown;

    private void Awake()
    {
        _killable = GetComponent<Killable>();
        _killable.OnDamage += ShieldRegenerator_OnDamage;
    }

    private void Update()
    {
        if (_delayCooldown >= 0f)
        {
            _delayCooldown -= Time.deltaTime;
        }
        if (_delayCooldown < 0f)
        {
            if (_killable.Shield < _killable.MaxShield)
            {
                _killable.Shield += RegenerationRate*Time.deltaTime;
                if (_killable.Shield > _killable.MaxShield)
                    _killable.Shield = _killable.MaxShield;
                if (OnRegenerate != null)
                    OnRegenerate();
            }
        }
    }

    private void ShieldRegenerator_OnDamage(Vector3 position, Vector3 normal, GameObject attacker)
    {
        _delayCooldown = RegenerationDelay;
    }
}
