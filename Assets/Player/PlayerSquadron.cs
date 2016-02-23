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
	public Texture2D ArrowCursorImage;
	public Texture2D TrackerCurosrImage;
	public Texture2D FarTrackerCursorImage;
	public Texture2D VeryFarTrackerCursorImage;
	public Texture2D LockingTrackerCursorImage;
	public Texture2D LockedTrackerCursorImage;
	public Color TrackerColor = Color.white;
	public Font SquadronTrackerFont;

	public GameObject SquadronPinPrefab;

	private int _curSquadronIndex;

	public void Initialize()
	{
		var playerVehicleInstance = PlayerController.Current.VehicleInstance;
		var playerNpc = PlayerController.Current.GetComponent<Fighter>();

		Members.Insert(0, playerNpc);

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

		var squadronTracker = member.VehicleInstance.gameObject.AddComponent<SquadronTracker>();
		squadronTracker.ArrowCursorImage = memberTracker.ArrowCursorImage;
		squadronTracker.TrackerCursorImage = memberTracker.TrackerCursorImage;
		squadronTracker.FarTrackerCursorImage = memberTracker.FarTrackerCursorImage;
		squadronTracker.VeryFarTrackerCursorImage = memberTracker.VeryFarTrackerCursorImage;
		squadronTracker.LockingCursorImage = memberTracker.LockingCursorImage;
		squadronTracker.LockedCursorImage = memberTracker.LockedCursorImage;
		Destroy(memberTracker);
		squadronTracker.CallSign = member.CallSign;
		squadronTracker.TrackerColor = TrackerColor;
		squadronTracker.LabelFont = SquadronTrackerFont;
		squadronTracker.IsDisabled = false;
		member.IsFollowIdleDestination = true;
		var mapPin = member.VehicleInstance.gameObject.AddComponent<MapPin>();
		mapPin.ActivePin = SquadronPinPrefab;
		mapPin.InactivePin = SquadronPinPrefab;
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

	private void PickupCollectibles(Fighter member)
	{
		var hitColliders = Physics.OverlapSphere(member.VehicleInstance.transform.position, CollectRadius, LayerMask.GetMask("Collectible"));
		foreach (var hitCollider in hitColliders)
		{
			var collectible = hitCollider.GetComponent<CollectibleTrigger>();
			if (collectible != null)
				collectible.Pickup(member.VehicleInstance.gameObject, member.VehicleInstance.GetVelocity());
		}
	}

	private void BindMemberEvents(Fighter member)
	{
		var squadronShieldRegenerator = member.VehicleInstance.GetComponent<ShieldRegenerator>();
		squadronShieldRegenerator.OnRegenerate += SquadronMember_OnRegenerate;
		var memberKillable = member.VehicleInstance.Killable;
		memberKillable.OnDamage += SquadronMember_OnDamage;
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
				CommMessaging.Current.ShowMessage(attacker, member.CallSign, GetFriendlyFireMessage(PlayerController.Current.GetCallSign()));
			}
		}
		// This should on refresh the current squadron member's icon.
		HeadsUpDisplay.Current.RefreshSquadronIcons();
	}

	private string GetFriendlyFireMessage(string addressCallSign)
	{
		var friendlyFireMessages = Dialogue.DialogueDictionary["FriendlyFire"];
		return string.Format(friendlyFireMessages[Random.Range(0, friendlyFireMessages.Count)], addressCallSign);
	}

	private void SquadronMember_OnDie(Killable sender)
	{
		PlayerController.Current.ResetThreatCooldown();
		// This should on refresh the current squadron member's icon.
		HeadsUpDisplay.Current.RefreshSquadronIcons();
		sender.OnDamage -= SquadronMember_OnDamage;
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
		return Members[_curSquadronIndex];
	}

	public int GetCurrentIndex()
	{
		return _curSquadronIndex;
	}
}
