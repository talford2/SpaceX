using UnityEngine;

public class SpaceBox : MonoBehaviour
{
    public Shiftable Shiftable;
    public Killable Killable;
    public Rigidbody RBody;

    public float InitialAngularSpeed = 90f;

    public GameObject DropItem;
    public int DropCount;

    private Vector3 _velocity;
    private Vector3 _angularVelocity;

    private void Awake()
    {
        Killable.OnDie += OnDie;
        SetAngularVelocity(Random.onUnitSphere * InitialAngularSpeed);
    }

    public void SetVelocity(Vector3 value)
    {
        _velocity = value;
    }

    public void SetAngularVelocity(Vector3 value)
    {
        _angularVelocity = value;
    }

    private void Update()
    {
        var displacement = _velocity * Time.deltaTime;
        var angularDisplacement = _angularVelocity * Time.deltaTime;

        RBody.rotation *= Quaternion.Euler(angularDisplacement);
        Shiftable.Translate(displacement);
    }

    private void OnDie(Killable sender)
    {
        Killable.OnDie -= OnDie;
        for (var i = 0; i < DropCount; i++)
        {
            var dropPosition = transform.position + Random.onUnitSphere * 1.5f;
            var fromCentre = dropPosition - transform.position;
            var dropItem = ((GameObject)Instantiate(DropItem, transform.position + Random.onUnitSphere * 1.5f, Quaternion.identity)).GetComponent<Collectible>();
            dropItem.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(dropPosition));
            dropItem.SetVelocity(_velocity + fromCentre.normalized * Random.Range(5f, 15f));
        }
    }
}
