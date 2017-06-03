using System.Collections.Generic;
using UnityEngine;

public class ProximitySpawner : MonoBehaviour
{
    public Fighter FighterPrefab;
    public float Radius = 2000f;
    public float Interval = 20f;
    public float IntervalIncreasePerKill = 0f;
    public int MaxLiveCount = 2;
    public Transform PathPoint;

    private float _radiusSquared;
    private float _intervalCooldown;

    private List<Killable> _spawnedKillables;
    private int _liveCount;

    private void Awake()
    {
        _radiusSquared = Radius*Radius;
        _intervalCooldown = Random.Range(0f, Interval);
        _liveCount = 0;
        _spawnedKillables = new List<Killable>();
    }

    private void Update()
    {
        if (PlayerController.Current != null && PlayerController.Current.VehicleInstance != null)
        {
            if (_intervalCooldown >= 0f)
            {
                _intervalCooldown -= Time.deltaTime;
                if (_intervalCooldown < 0f)
                {
                    var toPlayer = PlayerController.Current.VehicleInstance.transform.position - transform.position;
                    if (toPlayer.sqrMagnitude < _radiusSquared)
                    {
                        if (_liveCount < MaxLiveCount)
                        {
                            _liveCount++;
                            Spawn();
                        }
                    }
                    _intervalCooldown = Interval;
                }
            }
        }
    }

    public void Spawn()
    {
        var fighterInst = Instantiate(FighterPrefab);
        fighterInst.SpawnVehicle(fighterInst.gameObject, fighterInst.VehiclePrefab, Universe.Current.GetUniversePosition(transform.position), transform.rotation);
        var spawnedKillable = fighterInst.VehicleInstance.GetComponent<Killable>();
        spawnedKillable.OnDie += OnSpawnedDie;
        _spawnedKillables.Add(spawnedKillable);
        if (PathPoint != null)
        {
            fighterInst.SetPath(Universe.Current.GetUniversePosition(PathPoint.position));
        }
    }

    private void OnSpawnedDie(Killable sender, GameObject attacker)
    {
        _spawnedKillables.Remove(sender);
        _liveCount--;
        _intervalCooldown += IntervalIncreasePerKill;
        Debug.Log("SPAWNED DIED!");
    }

    private void OnDestroy()
    {
        foreach(var spawned in _spawnedKillables)
        {
            spawned.OnDie -= OnSpawnedDie;
        }
    }
}