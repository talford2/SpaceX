using UnityEngine;

public class SplashDamage {

	public static void ExplodeAt(Vector3 position, float maxRadius, float minRadius, float maxDamage, float force, int mask, GameObject attacker)
    {
        var _damageColliders = new Collider[10];
        var count = Physics.OverlapSphereNonAlloc(position, maxRadius, _damageColliders, mask);
        for (var i = 0; i < count; i++)
        {
            var detectable = _damageColliders[i].GetComponent<Detectable>();
            if (detectable != null)
            {
                var killable = detectable.TargetTransform.GetComponent<Killable>();
                if (killable != null)
                {
                    var damage = Mathf.Round(maxDamage * GetDamageFraction(detectable.transform.position, position, minRadius, maxRadius));
                    killable.Damage(damage, position, Vector3.up, attacker);
                }
            }
            var rBody = _damageColliders[i].GetComponentInParent<Rigidbody>();
            if (rBody != null)
                rBody.AddExplosionForce(force, position, maxRadius, 0f, ForceMode.Impulse);
        }
    }

    private static float GetDamageFraction(Vector3 targetPosition, Vector3 damagePosition, float minDistance, float maxDistance)
    {
        var toDamage = targetPosition - damagePosition;
        if (toDamage.sqrMagnitude < minDistance * minDistance)
            return 1f;
        if (toDamage.sqrMagnitude > maxDistance * maxDistance)
            return 0f;
        return 1f - Mathf.Clamp((toDamage.magnitude - minDistance) / (maxDistance - minDistance), 0, 1f);
    }
}
