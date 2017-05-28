using System.Collections.Generic;
using UnityEngine;

public class SpaceBox : MonoBehaviour
{
    public Shiftable Shiftable;
    public Killable Killable;
    public Rigidbody RBody;

    public float InitialAngularSpeed = 90f;

    public List<GameObject> DropItems;

    public GameObject DropJunk;
    public int DropJunkMinCount;
    public int DropJunkMaxCount;

    private GameObject _dropItem;
    private Vector3 _velocity;
    private Vector3 _angularVelocity;

    private void Awake()
    {
        Killable.OnDie += OnDie;
        SetAngularVelocity(Random.onUnitSphere * InitialAngularSpeed);
        _dropItem = DropItems[Random.Range(0, DropItems.Count)];
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

    private void OnDie(Killable sender, GameObject attacker)
    {
        Killable.OnDie -= OnDie;
        // Drop valuable
        var dropItem = ((GameObject)Instantiate(_dropItem, transform.position, Quaternion.identity)).GetComponent<Collectible>();
        dropItem.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(transform.position));
        dropItem.SetVelocity(_velocity * Random.Range(-0.5f, 0.5f));

        // Drop junk
        var dropCount = Random.Range(DropJunkMinCount, DropJunkMaxCount);
        for (var i = 0; i < DropJunkMaxCount; i++)
        {
            var dropPosition = transform.position + Random.onUnitSphere * 1.5f;
            var fromCentre = dropPosition - transform.position;
            var dropJunk = ((GameObject)Instantiate(DropJunk, transform.position + Random.onUnitSphere * 1.5f, Quaternion.identity)).GetComponent<Collectible>();
            dropJunk.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(dropPosition));
            dropJunk.SetVelocity(_velocity + fromCentre.normalized * Random.Range(5f, 15f));
        }
    }
}
