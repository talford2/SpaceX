using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSquadron : MonoBehaviour
{
    public List<Fighter> Members;
    public float ShieldRegenerateDelay = 5f;
    public float ShieldRegenerateRate = 5f;
    public float CollectRadius = 50f;

    [Header("Trackers")]
    public VehicleTrackerValues TrackerOptions;
    /*
    public Sprite ArrowSprite;
    public Sprite TrackerSprite;
    public Sprite FarTrackerSprite;
    public Sprite VeryFarTrackerSprite;
    public Sprite LockingTrackerSprite;
    public Sprite LockedTrackerSprite;
    public Color TrackerColor = Color.white;
    */
    public Font SquadronTrackerFont;
    public GameObject TrackerPlanePrefab;

    public GameObject SquadronPinPrefab;

    private int _curSquadronIndex;

    private int _collectableMask;

    public void Awake()
    {
        _collectableMask = LayerMask.GetMask("Collectible");
    }

    public void AddPlayerToSquadron()
    {
        Members.Insert(0, PlayerController.Current.GetComponent<Fighter>());
    }

    public void Initialize()
    {
        var playerNpc = PlayerController.Current.GetComponent<Fighter>();
        var playerVehicleInstance = PlayerController.Current.VehicleInstance;

        for (var i = 0; i < Members.Count; i++)
        {
            var member = Members[i];
            if (member != playerNpc)
            {
                var formationOffset = Formations.GetArrowOffset(i, 10f);
                var univPos = Universe.Current.GetUniversePosition(playerVehicleInstance.transform.position + playerVehicleInstance.transform.rotation * formationOffset);
                member.IsSquadronMember = true;

                // Give squadron members better aiming!
                member.MinAimOffsetRadius = 1.5f;
                member.MaxAimOffsetRadius = 5f;

                SpawnSquadronVehicle(member, univPos, transform.rotation);
            }
        }
        BindMemberEvents(playerNpc);
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
        member.SpawnVehicle(member.gameObject, member.VehiclePrefab, position, rotation);
        var memberTracker = member.VehicleInstance.GetComponent<VehicleTracker>();

        var profile = member.GetComponent<ShipProfile>();

        if (profile.PrimaryWeapon != null)
            member.VehicleInstance.SetPrimaryWeapon(profile.PrimaryWeapon.gameObject);
        if (profile.SecondaryWeapon != null)
            member.VehicleInstance.SetSecondaryWeapon(profile.SecondaryWeapon.gameObject);
        
        var squadronTracker = member.VehicleInstance.gameObject.AddComponent<SquadronTracker>();
        squadronTracker.Options = TrackerOptions;// memberTracker.Options;
        //squadronTracker.TrackerSprite = memberTracker.TrackerSprite;
        //squadronTracker.FarTrackerSprite = memberTracker.FarTrackerSprite;
        //squadronTracker.VeryFarTrackerSprite = memberTracker.VeryFarTrackerSprite;
        //squadronTracker.LockingSprite = memberTracker.LockingSprite;
        //squadronTracker.LockedSprite = memberTracker.LockedSprite;
        //squadronTracker.TrackerPlanePrefab = TrackerPlanePrefab;
        Destroy(memberTracker);
        squadronTracker.CallSign = profile.CallSign;
        //squadronTracker.TrackerColor = TrackerColor;
        squadronTracker.LabelFont = SquadronTrackerFont;
        squadronTracker.IsDisabled = false;
        member.IsFollowIdleDestination = true;
        var mapPin = member.VehicleInstance.gameObject.AddComponent<MapPin>();
        mapPin.ActivePin = PlayerController.Current.PlayerPinPrefab;
        mapPin.InactivePin = SquadronPinPrefab;
        mapPin.SetPinState(MapPin.MapPinState.Inactive);

        // Apply power profile
        member.VehicleInstance.Killable.MaxShield = profile.GetShield();
        member.VehicleInstance.Killable.Shield = member.VehicleInstance.Killable.MaxShield;

        var squadronShieldRegenerator = member.VehicleInstance.gameObject.AddComponent<ShieldRegenerator>();
        squadronShieldRegenerator.RegenerationDelay = ShieldRegenerateDelay;
        squadronShieldRegenerator.RegenerationRate = ShieldRegenerateRate;

        BindMemberEvents(member);

        member.enabled = true;
    }

    private void Update()
    {
        foreach (var member in Members)
        {
            if (member.VehicleInstance != null)
            {
                PickupCollectibles(member);
            }
        }
    }

    private CollectibleTrigger _collectibleTrigger;
    private Collider[] _hitColliders = new Collider[10];

    private void PickupCollectibles(Fighter member)
    {
        // TODO: Use non allocated Physics.OverlapSphereNonAlloc
        //hitColliders = Physics.OverlapSphere(member.VehicleInstance.transform.position, CollectRadius, _collectableMask);
        var count = Physics.OverlapSphereNonAlloc(member.VehicleInstance.transform.position, CollectRadius, _hitColliders, _collectableMask);

        for (var i = 0; i < count; i++)
        {
            _collectibleTrigger = _hitColliders[i].GetComponent<CollectibleTrigger>();
            if (_collectibleTrigger != null)
                _collectibleTrigger.Pickup(member.VehicleInstance.gameObject, member.VehicleInstance.GetVelocity());
        }
    }

    private void BindMemberEvents(Fighter member)
    {
        Debug.LogFormat("MEMBER: {0}", member.VehicleInstance);
        var squadronShieldRegenerator = member.VehicleInstance.GetComponent<ShieldRegenerator>();
        squadronShieldRegenerator.OnRegenerate += SquadronMember_OnRegenerate;
        var memberKillable = member.VehicleInstance.Killable;
        memberKillable.OnDamage += SquadronMember_OnDamage;
        memberKillable.OnLostShield += SquadronMember_OnLostShield;
        memberKillable.OnDie += SquadronMember_OnDie;
    }

    private void SquadronMember_OnRegenerate()
    {
        // This should on refresh the current squadron member's icon.
        HeadsUpDisplay.Current.RefreshSquadronIcons();
    }

    private void SquadronMember_OnDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (PlayerController.Current.VehicleInstance != null)
        {
            if (attacker == PlayerController.Current.VehicleInstance.gameObject)
            {
                var member = sender.GetComponent<Vehicle>().Controller.GetComponent<Fighter>();
                var profile = member.GetComponent<ShipProfile>();
                CommMessaging.Current.ShowMessage(attacker, profile.CallSign, GetFriendlyFireMessage(PlayerController.Current.GetCallSign(), profile.CallSign));
            }
        }
        // This should on refresh the current squadron member's icon.
        HeadsUpDisplay.Current.RefreshSquadronIcons();
    }

    private void SquadronMember_OnLostShield(Killable sender, GameObject attacker)
    {
        if (Random.value > 0.6f)
        {
            var playerVehicle = PlayerController.Current.VehicleInstance;
            if (playerVehicle != null)
            {
                if (attacker != playerVehicle.gameObject)
                {
                    var member = sender.GetComponent<Vehicle>().Controller.GetComponent<Fighter>();
                    var profile = member.GetComponent<ShipProfile>();
                    CommMessaging.Current.ShowMessage(playerVehicle.gameObject, profile.CallSign, GetLostShieldsMessage());
                }
            }
        }
    }

    private string GetFriendlyFireMessage(string toCallSign, string fromCallSign)
    {
        return Dialogue.GetRandomDialogue("FriendlyFire", toCallSign, fromCallSign);
    }

    private string GetLostShieldsMessage()
    {
        return Dialogue.GetRandomDialogue("LostShield");
    }

    private void SquadronMember_OnDie(Killable sender)
    {
        PlayerController.Current.ResetThreatCooldown();
        // This should on refresh the current squadron member's icon.
        HeadsUpDisplay.Current.RefreshSquadronIcons();
        sender.OnDamage -= SquadronMember_OnDamage;
        sender.OnLostShield -= SquadronMember_OnLostShield;
        sender.OnDie -= SquadronMember_OnDie;
        var shieldRegenerator = sender.GetComponent<ShieldRegenerator>();
        if (shieldRegenerator != null)
            shieldRegenerator.OnRegenerate -= SquadronMember_OnRegenerate;
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
        return GetMember(_curSquadronIndex);
    }

    public int GetCurrentIndex()
    {
        return _curSquadronIndex;
    }
}