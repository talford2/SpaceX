using System.Collections;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    public float MaxAngle = 15f;
    public float Frequency = 1.0f;

    private float maxAmplitude;
    private float shakeTheta;
    private float amplitudeTarget;
    private float amplitudeFraction;

    public void TriggerShake(float amplitude)
    {
        maxAmplitude = amplitude;
        shakeTheta = 0f;
        amplitudeTarget = 1f;
    }

    public void TriggerShake(float amplitude, float duration)
    {
        maxAmplitude = amplitude;
        shakeTheta = Random.Range(0f, 2f * Mathf.PI);
        amplitudeTarget = 1f;

        StartCoroutine(TimedShake(duration));
    }

    private IEnumerator TimedShake(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReleaseShake();
    }

    public void ReleaseShake()
    {
        amplitudeTarget = 0f;
    }

    public void UpdateShake(Transform applyToTransform)
    {
        amplitudeFraction = Mathf.MoveTowards(amplitudeFraction, amplitudeTarget, Time.deltaTime);

        shakeTheta = (shakeTheta + Frequency * Time.deltaTime) % (2f * Mathf.PI);

        var freq = shakeTheta * Mathf.Rad2Deg * 0.5f;

        var x = Mathf.Sin(freq);
        var y = Mathf.Sin(freq * 1.2f);
        var z = Mathf.Sin(freq * 1.4f);

        var fracVec = new Vector3(x, y, z) * maxAmplitude * amplitudeFraction;

        //transform.position += fracVec;
        //transform.position += Mathf.PerlinNoise(x, y)*Vector3.one * amplitudeFraction;
        applyToTransform.localRotation *= Quaternion.Euler(fracVec * MaxAngle);

    }
}

