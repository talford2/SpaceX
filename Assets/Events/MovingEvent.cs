using UnityEngine;

public class MovingEvent : MonoBehaviour
{
    public Shiftable Shiftable;
    public float Speed = 10f;

    private void Update()
    {
        Shiftable.Translate(Speed * transform.forward * Time.deltaTime);
    }
}
