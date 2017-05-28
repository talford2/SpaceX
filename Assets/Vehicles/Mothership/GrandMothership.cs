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
    [Header("Other")]
    public MapPin MapPin;
    public GameObject SoundPrefab;
    public AudioClip DefeatedSound;

    private int _liveCount;
    private List<Killable> _killables;

    private void Awake()
    {
        _killables = new List<Killable>();
        foreach (Transform turretTransform in LaserTurretGroupTransform)
        {
            var turret = Instantiate(LaserTurretPrefab, turretTransform.position, turretTransform.rotation).GetComponent<Turret>();
            turret.Targetable.Team = Team;
            turret.transform.SetParent(turretTransform);
            turret.transform.up = turretTransform.forward;
            _killables.Add(turret.GetComponent<Killable>());
        }
        foreach (Transform turretTransform in FlakTurretGroupTransform)
        {
            var turret = Instantiate(FlakTurretPrefab, turretTransform.position, turretTransform.rotation).GetComponent<Turret>();
            turret.Targetable.Team = Team;
            turret.transform.SetParent(turretTransform);
            turret.transform.up = turretTransform.forward;
            _killables.Add(turret.GetComponent<Killable>());
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
