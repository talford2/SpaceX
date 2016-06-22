using UnityEngine;

public class MovingEvent : MonoBehaviour
{
    public Shiftable Shiftable;
    public float Speed = 10f;

    public bool DestroyOverDistance;
    public float DestroyDistance;

    private float destroyDistSquared;

    private void Awake()
    {
        destroyDistSquared = DestroyDistance * DestroyDistance;
    }

    private void Update()
    {
        var distSquared = Shiftable.GetAbsoluteUniversePosition().sqrMagnitude;
        if (distSquared > destroyDistSquared)
        {
            Destroy(gameObject);
            return;
        }
        Shiftable.Translate(Speed * transform.forward * Time.deltaTime);
    }
}
