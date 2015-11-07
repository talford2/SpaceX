using UnityEngine;

public class IdleRotation : MonoBehaviour
{
    public float RotationSpeed;

    private Shiftable _shiftable;
    private Vector3 _rotationVelocity;

    private void Awake()
    {
        transform.rotation *= Quaternion.Euler(RandomEuler(360f));
        _shiftable = GetComponent<Shiftable>();
        _rotationVelocity = RandomEuler(RotationSpeed);
    }

    private void Update()
    {
        if (PlayerController.Current.InPlayerActiveCells(_shiftable.UniverseCellIndex))
        {
            transform.rotation *= Quaternion.Euler(_rotationVelocity * Time.deltaTime);
        }
    }

    private Vector3 RandomEuler(float magnitude)
    {
        var halfMagnitude = magnitude/2f;
        return new Vector3(Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude));
    }
}
