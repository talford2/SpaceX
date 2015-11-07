using UnityEngine;

public class IdleRotation : MonoBehaviour
{
    public float RotationSpeed;

    private Vector3 rotationVelocity;

    private void Awake()
    {
        transform.rotation *= Quaternion.Euler(RandomEuler(360f));
        rotationVelocity = RandomEuler(RotationSpeed);
    }

    private void Update()
    {
        transform.rotation *= Quaternion.Euler(rotationVelocity*Time.deltaTime);
    }

    private Vector3 RandomEuler(float magnitude)
    {
        var halfMagnitude = magnitude/2f;
        return new Vector3(Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude));
    }
}
