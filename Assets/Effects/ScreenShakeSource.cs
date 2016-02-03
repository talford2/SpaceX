using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class ScreenShakeSource : MonoBehaviour
{
    public float MaxAmplitude = 1f;
    public float MinDistance = 100f;
    public float MaxDistance = 1000f;
    public float Frequency = 0.7f;
    public float Duration = 1f;

    private void Start()
    {
        var amplitude = MaxAmplitude*GetAmplitudeFraction(Universe.Current.ViewPort.transform.position, transform.position, MinDistance, MaxDistance);
        Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(amplitude, Frequency, Duration);
    }

    private float GetAmplitudeFraction(Vector3 targetPosition, Vector3 emitterPosition, float minDistance, float maxDistance)
    {
        var toDamage = targetPosition - emitterPosition;
        if (toDamage.sqrMagnitude < minDistance * minDistance)
            return 1f;
        if (toDamage.sqrMagnitude > maxDistance * maxDistance)
            return 0f;
        return 1f - Mathf.Clamp((toDamage.magnitude - minDistance) / (maxDistance - minDistance), 0, 1f);
    }
}
