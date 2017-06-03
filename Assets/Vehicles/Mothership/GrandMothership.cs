using System.Collections.Generic;
using UnityEngine;

public class GrandMothership : MonoBehaviour
{
    public Team Team;
    [Header("Laser Turrets")]
    public GameObject LaserTurretPrefab;
    public Transform LaserTurretGroupTransform;
    //public List<Transform> LaserTurretTransforms;
    [Header("Flak Turrets")]
    public GameObject FlakTurretPrefab;
    public Transform FlakTurretGroupTransform;
    //public List<Transform> FlakTurretTransforms;

    [Header("Tanks")]
    public GameObject TankPrefab;
    public Transform TankGroupTransform;

    [Header("Domes")]
    public GameObject DomePrefab;
    public Transform DomeGroupTransform;

    [Header("Spawners")]
    public Transform SpawnersGroupTransform;

    [Header("Other")]
    public MapPin MapPin;
    public GameObject SoundPrefab;
    public AudioClip DefeatedSound;

    private int _liveCount;
    private List<Killable> _killables;
    private List<ProximitySpawner> _spawners;

    private void Awake()
    {
        _killables = new List<Killable>();
        foreach (Transform turretTransform in LaserTurretGroupTransform)
        {
            Destroy(turretTransform.GetComponent<MeshRenderer>());
            var turret = Instantiate(LaserTurretPrefab, turretTransform.position, turretTransform.rotation).GetComponent<Turret>();
            turret.Targetable.Team = Team;
            turret.transform.SetParent(turretTransform);
            turret.transform.up = turretTransform.forward;
            //_killables.Add(turret.GetComponent<Killable>());
        }
        foreach (Transform turretTransform in FlakTurretGroupTransform)
        {
            Destroy(turretTransform.GetComponent<MeshRenderer>());
            var turret = Instantiate(FlakTurretPrefab, turretTransform.position, turretTransform.rotation).GetComponent<Turret>();
            turret.Targetable.Team = Team;
            turret.transform.SetParent(turretTransform);
            turret.transform.up = turretTransform.forward;
            //_killables.Add(turret.GetComponent<Killable>());
        }
        foreach(Transform tankTransform in TankGroupTransform)
        {
            Destroy(tankTransform.GetComponent<MeshRenderer>());
            var tank = Instantiate(TankPrefab, tankTransform.position, tankTransform.rotation);
            tank.transform.SetParent(tankTransform);
            tank.transform.localPosition = Vector3.zero;
            tank.transform.localRotation = Quaternion.identity;
            _killables.Add(tank.GetComponent<Killable>());
        }
        foreach (Transform domeTransform in DomeGroupTransform)
        {
            Destroy(domeTransform.GetComponent<MeshRenderer>());
            var dome = Instantiate(DomePrefab, domeTransform.position, domeTransform.rotation);
            dome.transform.SetParent(domeTransform);
            dome.transform.localPosition = Vector3.zero;
            dome.transform.localRotation = Quaternion.identity;
            _killables.Add(dome.GetComponent<Killable>());
        }
        _spawners = new List<ProximitySpawner>();
        foreach(Transform spawnerTransform in SpawnersGroupTransform)
        {
            var spawner = spawnerTransform.GetComponentInChildren<ProximitySpawner>();
            _spawners.Add(spawner);
        }
        _liveCount = _killables.Count;
        foreach (var killable in _killables)
        {
            killable.OnDie += OnKill;
        }
    }

    private void OnKill(Killable sender, GameObject attacker)
    {
        _liveCount--;
        Debug.Log("MOTHERSHIP LIVE COUNT: " + _liveCount);
        if (_liveCount <= 0)
        {
            Debug.Log("MOTHERSHIP DESTROYED!!!");
            HeadsUpDisplay.Current.DisplayMessage("MOTHERSHIP NEUTRALIZED", 3f);
            foreach (var spawner in _spawners)
            {
                if (spawner != null)
                    Destroy(spawner.gameObject);
            }
            if (DefeatedSound != null)
            {
                ResourcePoolManager.GetAvailable(SoundPrefab, Universe.Current.ViewPort.transform.position, Quaternion.identity)
                    .GetComponent<AnonymousSound>()
                    .PlayAt(DefeatedSound, Universe.Current.ViewPort.transform.position, 1f, false);
            }
            MapPin.SetPinState(MapPin.MapPinState.Inactive);
        }
    }
}
