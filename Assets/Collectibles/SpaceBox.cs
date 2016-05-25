using UnityEngine;

public class SpaceBox : MonoBehaviour
{
    public Shiftable Shiftable;

    private Vector3 _velocity;
    private Vector3 _angularVelocity;

    public void SetVelocity(Vector3 value)
    {
        _velocity = value;
    }

    public void SetAngularVelocity(Vector3 value)
    {

    }

    private void Update()
    {
        var displacement = _velocity * Time.deltaTime;
        var angularDisplacement = _angularVelocity * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(angularDisplacement);
        Shiftable.Translate(displacement);
    }
}
