using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSquadron : MonoBehaviour
{
    public List<Fighter> Members;

    [Header("Trackers")]
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCurosrImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;
    public Texture2D LockingTrackerCursorImage;
    public Texture2D LockedTrackerCursorImage;
    public Color TrackerColor = Color.white;

    public GameObject SquadronPinPrefab;

    private int _curSquadronIndex;

    public void Initialize()
    {
        var playerVehicleInstance = PlayerController.Current.VehicleInstance;
        var playerNpc = PlayerController.Current.GetComponent<Fighter>();

        for (var i = 0; i < Members.Count; i++)
        {
            var member = Members[i];
            if (member != playerNpc)
            {
                var univPos = playerVehicleInstance.Shiftable.UniversePosition;
                univPos.CellLocalPosition += Formations.GetArrowOffset(i, 10f);
                member.IsSquadronMember = true;

                // Give squadron members better aiming!
                member.AimOffsetRadius = 1.5f;
                SpawnSquadronVehicle(member, univPos, transform.rotation);
            }
        }

        Members.Insert(0, playerNpc);
    }

    public int CycleSquadronIndex(int dir)
    {
        if (Members.Any(s => s.VehicleInstance != null))
        {
            _curSquadronIndex += dir;
            if (_curSquadronIndex >= Members.Count)
                _curSquadronIndex = 0;
            if (_curSquadronIndex < 0)
                _curSquadronIndex = Members.Count - 1;
            if (Members[_curSquadronIndex].VehicleInstance == null)
                return CycleSquadronIndex(dir);
            return _curSquadronIndex;
        }
        return -1;
    }

    public void SpawnSquadronVehicle(Fighter member, UniversePosition position, Quaternion rotation)
    {
        member.SpawnVehicle(member.VehiclePrefab, position, rotation);
        var memberTracker = member.VehicleInstance.GetComponent<VehicleTracker>();
        memberTracker.ArrowCursorImage = ArrowCursorImage;
        memberTracker.TrackerCursorImage = TrackerCurosrImage;
        memberTracker.FarTrackerCursorImage = FarTrackerCursorImage;
        memberTracker.VeryFarTrackerCursorImage = VeryFarTrackerCursorImage;
        memberTracker.LockingCursorImage = LockingTrackerCursorImage;
        memberTracker.LockedCursorImage = LockedTrackerCursorImage;
        memberTracker.TrackerColor = TrackerColor;
        memberTracker.CallSign = member.CallSign;
        member.IsFollowIdleDestination = true;
        var mapPin = member.VehicleInstance.gameObject.AddComponent<MapPin>();
        mapPin.ActivePin = SquadronPinPrefab;
        mapPin.InactivePin = SquadronPinPrefab;
        var squadronHealthRegenerator = member.VehicleInstance.gameObject.AddComponent<HealthRegenerator>();
        squadronHealthRegenerator.RegenerationDelay = 5f;
        squadronHealthRegenerator.RegenerationRate = 5f;
        var memberKillable = member.VehicleInstance.GetComponent<Killable>();
        memberKillable.OnDamage += SquadronMember_OnDamage;
        memberKillable.OnDie += SquadronMember_OnDie;
        member.enabled = true;
    }

    private void SquadronMember_OnDamage(Vector3 position, Vector3 normal, GameObject attacker)
    {
        HeadsUpDisplay.Current.RefreshSquadronIcons();
    }

    private void SquadronMember_OnDie(Killable sender)
    {
        HeadsUpDisplay.Current.RefreshSquadronIcons();
        sender.OnDamage -= SquadronMember_OnDamage;
        sender.OnDie -= SquadronMember_OnDie;
    }

    public int GetLiveCount()
    {
        return Members.Count(s => s.VehicleInstance != null);
    }

    public int GetMemberCount()
    {
        return Members.Count;
    }

    public Fighter GetMember(int index)
    {
        return Members[index];
    }

    public Fighter GetCurrentMember()
    {
        return Members[_curSquadronIndex];
    }

    public int GetCurrentIndex()
    {
        return _curSquadronIndex;
    }
}
