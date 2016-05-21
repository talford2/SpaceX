using UnityEngine;
using System.Collections.Generic;

public class Freighter : MonoBehaviour
{
    public List<Transform> Containers;
    public float ContainerHealth = 50f;

    [Header("Container Drop Items")]
    public GameObject DropItemPrefab;
    public int MinDropCount = 3;
    public int MaxDropCount = 10;

    private bool hasBeenShot;

    void Start()
    {
        foreach(var container in Containers)
        {
            var killable = container.GetComponent<Killable>();
            if (killable!=null)
            {
                killable.MaxHealth = ContainerHealth;
                killable.Health = killable.MaxHealth;
                killable.OnDamage += OnDamage;
                killable.OnDie += OnContainerDie;
            }
        }
        hasBeenShot = false;
    }

    private void OnDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (!hasBeenShot)
        {
            hasBeenShot = true;
            // Trigger enemies
        }
    }

    private void OnContainerDie(Killable sender)
    {
        var dropAmount = Random.Range(MinDropCount, MaxDropCount + 1);
        for (var i = 0f; i < dropAmount; i++)
        {
            var dropPosition = sender.transform.position + Random.onUnitSphere * 1.5f;
            var collectible = DropItemPrefab;
            var dropItem = ((GameObject)Instantiate(collectible, dropPosition, Quaternion.identity)).GetComponent<Collectible>();
            dropItem.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(dropPosition));
            dropItem.SetVelocity(Random.onUnitSphere * 5f);
        }
    }
}
