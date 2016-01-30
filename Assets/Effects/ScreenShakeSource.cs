using UnityEngine;

public class ScreenShakeSource : MonoBehaviour
{
    public float MaxAmplitude = 1f;
    public float MinDistance = 100f;
    public float MaxDistance = 1000f;
    public float Frequency = 0.7f;
    public float Duration = 1f;

    private void Awake()
    {
        var amplitude = GetAmplitude(transform.position, MaxAmplitude, MinDistance, MaxDistance);
        Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(amplitude, Frequency, Duration);
    }

    private float GetAmplitude(Vector3 position, float maxAmplitude, float minDistance, float maxDistance)
    {
        var dist = (position - Universe.Current.ViewPort.transform.position).magnitude;
        if (dist < minDistance)
            return maxAmplitude;
        if (dist < maxDistance)
            return maxAmplitude*Mathf.Clamp((dist - minDistance)/maxDistance, 0f, 1f);
        return 0f;
    }
}
