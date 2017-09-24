using UnityEngine;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    public Team Team;
    public GameObject TurretPrefab;
    public List<Transform> TurretTransforms;
    public List<Transform> BayTransforms;
    public MapPin MapPin;
    public GameObject SoundPrefab;
    public AudioClip DefeatedSound;

    [Header("Bays/Shields")]
    public Killable FrontLeftBay;
    public MotherhsipBayDoors FrontLeftDoors;
    public MeshRenderer FrontLeftBayShield;
    public MeshRenderer DistantFrontLeftBayShield;

    public Killable FrontRightBay;
    public MotherhsipBayDoors FrontRightDoors;
    public MeshRenderer FrontRightBayShield;
    public MeshRenderer DistantFrontRightBayShield;

    public Killable MidLeftBay;
    public MotherhsipBayDoors MidLeftDoors;
    public Killable RearLeftBay;
    public MeshRenderer MidLeftBayShield;
    public MeshRenderer DistantMidLeftBayShield;

    public Killable MidRightBay;
    public MotherhsipBayDoors MidRightDoors;
    public Killable RearRightBay;
    public MeshRenderer MidRightBayShield;
    public MeshRenderer DistantMidRightBayShield;

    private int _liveCount;
    private List<Killable> _killables;

    private void Start()
    {
        _killables = new List<Killable>();
        foreach (var turretTransform in TurretTransforms)
        {
            var turret = ((GameObject)Instantiate(TurretPrefab, turretTransform.position, turretTransform.rotation)).GetComponent<Turret>();
            turret.Targetable.Team = Team;
            turret.transform.SetParent(turretTransform);
            _killables.Add(turret.GetComponent<Killable>());
        }
        foreach (var bayTransform in BayTransforms)
        {
            var bayTargetable = bayTransform.GetComponent<Targetable>();
            bayTargetable.Team = Team;
            _killables.Add(bayTransform.GetComponent<Killable>());
        }
        _liveCount = _killables.Count;
        foreach (var killable in _killables)
        {
            killable.OnDie += OnKill;
        }

        FrontLeftBay.OnDie += (sender, positon, normal, attacker) =>
        {
            FrontLeftBayShield.enabled = false;
            DistantFrontLeftBayShield.enabled = false;
            FrontLeftDoors.TriggerClose();
        };
        FrontRightBay.OnDie += (sender, positon, normal, attacker) =>
        {
            FrontRightBayShield.enabled = false;
            DistantFrontRightBayShield.enabled = false;
            FrontRightDoors.TriggerClose();
        };

        MidLeftBay.OnDie += (sender, positon, normal, attacker) =>
        {
            if (RearLeftBay == null)
            {
                MidLeftBayShield.enabled = false;
                DistantMidLeftBayShield.enabled = false;
                MidLeftDoors.TriggerClose();
            }
        };
        RearLeftBay.OnDie += (sender, positon, normal, attacker) =>
        {
            if (MidLeftBay == null)
            {
                MidLeftBayShield.enabled = false;
                DistantMidLeftBayShield.enabled = false;
                MidLeftDoors.TriggerClose();
            }
        };

        MidRightBay.OnDie += (sender, positon, normal, attacker) =>
        {
            if (RearRightBay == null)
            {
                MidRightBayShield.enabled = false;
                DistantMidRightBayShield.enabled = false;
                MidRightDoors.TriggerClose();
            }
        };
        RearRightBay.OnDie += (sender, positon, normal, attacker) =>
        {
            if (MidRightBay == null)
            {
                MidRightBayShield.enabled = false;
                DistantMidRightBayShield.enabled = false;
                MidRightDoors.TriggerClose();
            }
        };
    }

    private void OnKill(Killable sender, Vector3 positon, Vector3 normal, GameObject attacker)
    {
        _liveCount--;
        Debug.Log("MOTHERSHIP LIVE COUNT: " + _liveCount);
        if (_liveCount <= 0)
        {
            Debug.Log("MOTHERSHIP DESTROYED!!!");
            HeadsUpDisplay.Current.DisplayMessage("MOTHERSHIP NEUTRALIZED", 3f);
            if (DefeatedSound != null)
            {
                var sound = ResourcePoolManager.GetAvailable(SoundPrefab, Universe.Current.ViewPort.transform.position, Quaternion.identity).GetComponent<AnonymousSound>();
                sound.PlayAt(DefeatedSound, Universe.Current.ViewPort.transform.position, 1f, false);
            }
            MapPin.SetPinState(MapPin.MapPinState.Inactive);
        }
    }
}