using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Freighter : MonoBehaviour
{
    public List<Transform> Containers;
    public List<Transform> DistantContainers;
    public float ContainerHealth = 50f;
    public List<Material> ContainerMaterials;

    [Header("Turrets")]
    public GameObject TurretPrefab;
    public List<Transform> TurretTransforms;

    [Header("Container Drop Items")]
    public GameObject DropItemPrefab;
    public int MinDropCount = 3;
    public int MaxDropCount = 10;

    [Header("Spawners")]
    public float SpawnDistance = 1000f;
    public int GroupCount = 3;
    public int GroupMemberCount = 7;
    public Shiftable Shiftable;
    public Spawner SpawnerPrefab;

    private bool hasBeenShot;
    private List<Turret> _turrets;

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
        _turrets = new List<Turret>();
        foreach (var turretTransform in TurretTransforms)
        {
            var turret = Instantiate(TurretPrefab, turretTransform.position, turretTransform.rotation).GetComponent<Turret>();
            turret.Targetable.Team = Team.Neutral;
            turret.transform.SetParent(turretTransform);
            _turrets.Add(turret);
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

            for (var i = 0; i < GroupCount; i++)
            {
                var fromPoint = transform.position + Random.onUnitSphere * SpawnDistance;
                var rotFacing = Quaternion.LookRotation(fromPoint - transform.position);
                for (var j = 0; j < GroupMemberCount; j++)
                {
                    var spawner = Instantiate(SpawnerPrefab);
                    spawner.Shifter = Shiftable;
                    spawner.transform.position = fromPoint + rotFacing * Formations.GetArrowOffset(j, 5f);
                    spawner.Spawn(i * 1.5f + Random.Range(0.2f, 0.5f));
                }
            }

            foreach(var turret in _turrets)
            {
                turret.Targetable.Team = Team.Bad;
            }
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
