using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class ScreenShakeSource : MonoBehaviour
{
    public float MaxAmplitude = 1f;
    public float MinDistance = 100f;
    public float MaxDistance = 1000f;
    public float Frequency = 0.7f;
    public float Duration = 1f;

    private Shiftable shiftable;

    private void Awake()
    {
        shiftable = GetComponent<Shiftable>();
        var amplitude = GetAmplitude(MaxAmplitude, MinDistance, MaxDistance);
        Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(amplitude, Frequency, Duration);
    }

    private float GetAmplitude(float maxAmplitude, float minDistance, float maxDistance)
    {
        var dist = (shiftable.GetWorldPosition() - Universe.Current.ViewPort.Shiftable.GetWorldPosition()).magnitude;
        if (dist < minDistance)
            return maxAmplitude;
        if (dist < maxDistance)
            return maxAmplitude*(1f - Mathf.Clamp((dist - minDistance)/maxDistance, 0f, 1f));
        return 0f;
    }
}
