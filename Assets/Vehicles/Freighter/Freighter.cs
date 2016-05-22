using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Freighter : MonoBehaviour
{
    public List<Transform> Containers;
    public List<Transform> DistantContainers;
    public float ContainerHealth = 50f;
    public List<Material> ContainerMaterials;

    [Header("Container Drop Items")]
    public GameObject DropItemPrefab;
    public int MinDropCount = 3;
    public int MaxDropCount = 10;

    [Header("Spawners")]
    public Shiftable Shiftable;
    public Spawner SpawnerPrefab;

    private bool hasBeenShot;

    void Start()
    {
        foreach (var container in Containers)
        {
            var killable = container.GetComponent<Killable>();
            if (killable != null)
            {
                killable.MaxHealth = ContainerHealth;
                killable.Health = killable.MaxHealth;
                killable.OnDamage += OnDamage;
                killable.OnDie += OnContainerDie;
            }
            var containerRenderer = container.GetComponent<MeshRenderer>();
            if (containerRenderer != null)
            {
                containerRenderer.material = ContainerMaterials[Random.Range(0, ContainerMaterials.Count)];
            }
        }
        // Match Materials
        for (var i = 0; i < Containers.Count; i++)
        {
            var distantContainerRenderer = DistantContainers[i].GetComponent<MeshRenderer>();
            distantContainerRenderer.material = Containers[i].GetComponent<MeshRenderer>().material;
        }
        hasBeenShot = false;
    }

    private void OnDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (!hasBeenShot)
        {
            hasBeenShot = true;

            for (var i = 0; i < 3; i++)
            {
                var fromPoint = transform.position + Random.onUnitSphere * 500f;
                var rotFacing = Quaternion.LookRotation(fromPoint - transform.position);
                for (var j = 0; j < 7; j++)
                {
                    var spawner = Instantiate(SpawnerPrefab);
                    spawner.Shifter = Shiftable;
                    spawner.transform.position = fromPoint + rotFacing * Formations.GetArrowOffset(j, 5f);
                    spawner.Spawn(i * 1.5f + Random.Range(0.2f, 0.5f));
                }
            }

            /*
            var fromPoint = transform.position + Random.onUnitSphere * 500f;
            var rotFacing = Quaternion.LookRotation(fromPoint - transform.position);

            for (var i = 0; i < Spawners.Count; i++)
            {
                Spawners[i].transform.position = fromPoint + rotFacing * Formations.GetArrowOffset(i, 5f);
            }
            foreach (var spawner in Spawners)
            {
                spawner.Spawn(Random.Range(0.2f, 0.5f), attacker.transform);
            }
            */
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
        var distantHitContainer = DistantContainers.First(c => c.name == sender.name);
        distantHitContainer.GetComponent<MeshRenderer>().enabled = false;
    }
}
