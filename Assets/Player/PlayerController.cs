using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Vehicle VehiclePrefab;
	public Team Team;
	public string CallSign;
	public int SpaceJunkCount;
	public int PowerNodeCount;
	public float CollectRadius = 8f;
    public float DoubleTapTime = 0.2f;

	private Vehicle _playVehicleInstance;
	private Fighter _playerNpc;
	public Color TrackerColor;

	public float ThreatRadius = 2000f;
	public PlayerSquadron Squadron;

	private bool _controlEnabled;

	private static PlayerController _current;

	public bool InvertY = false;
	public bool HideMouse = false;

	public Shiftable RespawnPosition;

	public GameObject PlayerPinPrefab;

	public delegate void OnPlayerControllerChangeSquadronMember(GameObject from, GameObject to);
	public OnPlayerControllerChangeSquadronMember OnChangeSquadronMember;

	[Header("Aiming")]
	//public float AimSensitivity = 10f;
	public float MouseMoveClamp = 1f;// 0.02f;

	public float DefaultAimDistance = 200f;
	public float MinAimDistance = 10f;
	public float MaxAimDistance = 1000f;

	[Header("Other")]
	public float NoThreatTime = 5f;
	public float DeathOptionTime = 1f;

	private float _aimDistance;
	private float _screenAspect;
	private int _threatCount;
	private float _threatCheckCooldown;
	private float _noThreatCooldown;

	private float _deathCooldown;
	private bool _isAllowRespawn;

	private int _aimMask;
	private int _detectableMask;
	private int _collectableMask;
	private UniversePosition _lastDeathUniversePosition;

    // Barrel Roll Trigger
    private int lastRollSign;
    private float lastLeftTime;
    private float lastRightTime;

    private void Awake()
	{
		_current = this;
		Cursor.visible = !HideMouse;
		if (HideMouse)
			Cursor.lockState = CursorLockMode.Locked;

		_screenAspect = (float)Screen.height / (float)Screen.width;

		_playerNpc = gameObject.AddComponent<Fighter>();
		_playerNpc.Team = Team;
		_playerNpc.IsSquadronMember = true;
		_playerNpc.GetComponent<ShipProfile>().CallSign = CallSign;
		_playerNpc.VehiclePrefab = VehiclePrefab;
		_playerNpc.enabled = false;
		SpawnVehicle(VehiclePrefab, Universe.Current.PlayerSpawnPosition);
		_playerNpc.SetVehicleInstance(_playVehicleInstance);
		_playerNpc.IsFollowIdleDestination = true;

		SetControlEnabled(true);

		_aimMask = ~LayerMask.GetMask("Player", "Detectable", "Distant");
		_detectableMask = LayerMask.GetMask("Detectable");
		_collectableMask = LayerMask.GetMask("Collectible");
	}

	private void Start()
	{
		Squadron.Initialize();

		HeadsUpDisplay.Current.LazyCreateSquadronIcons();
		HeadsUpDisplay.Current.RefreshSquadronIcons();

		CycleSquadron(0);
	}

	public void SpawnVehicle(Vehicle vehiclePrefab, Shiftable spawner)
	{
		SpawnVehicle(vehiclePrefab, spawner.UniversePosition, spawner.transform.rotation);
	}

	public void SpawnVehicle(Vehicle vehiclePrefab, UniversePosition universePosition, Quaternion rotation)
	{
		_playVehicleInstance = ((GameObject)Instantiate(vehiclePrefab.gameObject, universePosition.CellLocalPosition, rotation)).GetComponent<Vehicle>();
		_playVehicleInstance.Controller = gameObject;
		_playVehicleInstance.Shiftable.SetShiftPosition(universePosition);
		//Destroy(_playVehicleInstance.GetComponent<Tracker>());

		Destroy(_playVehicleInstance.GetComponent<VehicleTracker>());
		var squadronTracker = _playVehicleInstance.gameObject.AddComponent<SquadronTracker>();
		squadronTracker.ArrowSprite = Squadron.ArrowSprite;
		//squadronTracker.TrackerSprite = Squadron.TrackerSprite;
		//squadronTracker.FarTrackerSprite = Squadron.FarTrackerSprite;
		//squadronTracker.VeryFarTrackerSprite = Squadron.VeryFarTrackerSprite;
		squadronTracker.LockingSprite = Squadron.LockingTrackerSprite;
		squadronTracker.LockedSprite = Squadron.LockedTrackerSprite;
		squadronTracker.CallSign = CallSign;
		squadronTracker.TrackerColor = TrackerColor;
		squadronTracker.LabelFont = Squadron.SquadronTrackerFont;
		squadronTracker.IsDisabled = true;

		var mapPin = VehicleInstance.gameObject.AddComponent<MapPin>();
		mapPin.ActivePin = PlayerPinPrefab;
		mapPin.InactivePin = Squadron.SquadronPinPrefab;

		_playVehicleInstance.GetComponent<Targetable>().Team = Team;
		_playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");

		_playVehicleInstance.Killable.OnDamage += PlayerController_OnDamage;
		_playVehicleInstance.Killable.OnDie += PlayerController_OnDie;

		// Apply power profile
		var powerProfile = GetComponent<ShipProfile>();
		_playVehicleInstance.Killable.MaxShield = powerProfile.GetShield();
		_playVehicleInstance.Killable.Shield = _playVehicleInstance.Killable.MaxShield;
		_playVehicleInstance.MaxBoostEnergy = powerProfile.GetBoostEnergy();
		_playVehicleInstance.BoostEnergy = _playVehicleInstance.MaxBoostEnergy;

		var shieldRegenerator = _playVehicleInstance.gameObject.AddComponent<ShieldRegenerator>();
		shieldRegenerator.RegenerationDelay = Squadron.ShieldRegenerateDelay;
		shieldRegenerator.RegenerationRate = Squadron.ShieldRegenerateRate;
		shieldRegenerator.OnRegenerate += PlayerController_OnRegenerate;
	}

	private Vector3 GetAimAt()
	{
		var mouseRay = Universe.Current.ViewPort.AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit aimHit;
		_aimDistance = Mathf.Lerp(_aimDistance, DefaultAimDistance, Time.deltaTime);
		var aimAtPosition = mouseRay.GetPoint(DefaultAimDistance);

		// Fancy System.
		var viewPortPos = Universe.Current.ViewPort.transform.position;
		var viewPortForward = Universe.Current.ViewPort.transform.forward;
		var dotViewPort = VehicleInstance.PrimaryWeaponInstance != null
			? Vector3.Dot(VehicleInstance.PrimaryWeaponInstance.GetShootPointCentre() - viewPortPos, viewPortForward)
			: Vector3.Dot(VehicleInstance.transform.position - viewPortPos, viewPortForward);
		var guessTarget = Targeting.FindFacingAngleAny(viewPortPos + dotViewPort * viewPortForward, viewPortForward, MaxAimDistance, 5f);
		if (guessTarget != null)
		{
			var toGuessTarget = guessTarget.position - viewPortPos;
			_aimDistance = Mathf.Clamp(toGuessTarget.magnitude, MinAimDistance, MaxAimDistance);
			return mouseRay.GetPoint(_aimDistance);
		}

		if (Physics.Raycast(mouseRay, out aimHit, MaxAimDistance, _aimMask))
		{
			_aimDistance = Mathf.Clamp(aimHit.distance, MinAimDistance, MaxAimDistance);
			aimAtPosition = mouseRay.GetPoint(_aimDistance);
		}
		return aimAtPosition;
	}

	public void SetControlEnabled(bool value)
	{
		_controlEnabled = value;
	}

	private CollectibleTrigger _collectible;
	private int castCount = 0;
	private Collider[] hitColliders = new Collider[20];
	private int i = 0;

	private void PickupCollectibles()
	{
		castCount = Physics.OverlapSphereNonAlloc(VehicleInstance.transform.position, CollectRadius, hitColliders, _collectableMask);
		for (i = 0; i < castCount; i++)
		{
			_collectible = hitColliders[i].GetComponent<CollectibleTrigger>();
			if (_collectible != null)
				_collectible.Pickup(VehicleInstance.gameObject, VehicleInstance.GetVelocity());
		}
	}

	// Performance variables
	private Fighter _leader;
	private DroneHive _droneHive;
	private UniversePosition _spawnPos;
	private Collider[] colliders = new Collider[10];

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Debug.Break();
		}
		if (_controlEnabled)
		{
			if (_playVehicleInstance != null)
			{
				var mouseHorizontal = Input.GetAxis("MouseHorizontal");
				//AimSensitivity*Input.GetAxis("MouseHorizontal")/Screen.width;
				var mouseVertical = Input.GetAxis("MouseVertical");
				//AimSensitivity*_screenAspect*Input.GetAxis("MouseVertical")/Screen.height;

				var controllerHorizontal = 0f;// AimSensitivity*Input.GetAxis("Horizontal")/Screen.width;
				var controllerVertical = 0f;// AimSensitivity*_screenAspect*Input.GetAxis("Vertical")/Screen.height;

				var _pitchYaw = Vector2.ClampMagnitude(new Vector2(controllerVertical + mouseVertical, controllerHorizontal + mouseHorizontal), MouseMoveClamp);

				if (InvertY)
				{
					_playVehicleInstance.PitchThotttle = _pitchYaw.x * -1;
				}
				else
				{
					_playVehicleInstance.PitchThotttle = _pitchYaw.x;
				}
				_playVehicleInstance.YawThrottle = _pitchYaw.y;
				_playVehicleInstance.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
				if (_playVehicleInstance.PrimaryWeaponInstance != null)
					_playVehicleInstance.PrimaryWeaponInstance.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;
				if (_playVehicleInstance.SecondaryWeaponInstance != null)
					_playVehicleInstance.SecondaryWeaponInstance.IsTriggered = (Input.GetAxis("AltFireTrigger") + Input.GetAxis("MouseAltFireTrigger")) > 0;

				_playVehicleInstance.SetAimAt(GetAimAt());

                // Barrel roll trigger
                var curRollSign = Mathf.RoundToInt(_playVehicleInstance.RollThrottle);
                if (curRollSign != lastRollSign)
                {
                    if (lastRollSign == 0)
                    {
                        if (curRollSign == -1)
                        {
                            if (Time.time - lastLeftTime < DoubleTapTime)
                                _playVehicleInstance.TriggerBarrelRoll(curRollSign);
                            lastLeftTime = Time.time;
                        }
                        if (curRollSign == 1)
                        {
                            if (Time.time - lastRightTime < DoubleTapTime)
                                _playVehicleInstance.TriggerBarrelRoll(curRollSign);
                            lastRightTime = Time.time;
                        }
                    }
                }
                lastRollSign = curRollSign;

                _playVehicleInstance.TriggerAccelerate = false;
				if (Input.GetButton("Accelerate") || Input.GetButton("KeyboardAccelerate"))
				{
					_playVehicleInstance.TriggerAccelerate = true;
				}

				_playVehicleInstance.TriggerBrake = false;
				if (Input.GetButton("Brake") || Input.GetButton("KeyboardBrake"))
				{
					_playVehicleInstance.TriggerBrake = true;
				}

				_playVehicleInstance.TriggerBoost = false;
				if (Input.GetButton("Boost") || Input.GetButton("KeyboardBoost"))
				{
					_playVehicleInstance.TriggerBoost = true;
				}

				_droneHive = _playVehicleInstance.GetComponent<DroneHive>();
				if (_droneHive != null)
				{
					if (Input.GetKeyUp(KeyCode.T))
					{
						_droneHive.ReleaseDrones(5);
					}
				}

				if (_playVehicleInstance.IsBoosting)
				{
					//Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.04f, 1f);
				}

				PickupCollectibles();
				_isAllowRespawn = false;
			}
			else
			{
				if (_deathCooldown >= 0f)
				{
					_deathCooldown -= Time.deltaTime;
					if (_deathCooldown < 0f)
					{
						Debug.Log("ALLOW RESPAWN");
						_isAllowRespawn = true;
					}
				}
			}

			if (_isAllowRespawn)
			{
				if ((Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0)
				{
					Respawn();
				}
				if ((Input.GetAxis("AltFireTrigger") + Input.GetAxis("MouseAltFireTrigger")) > 0)
				{
					Respawn();
				}
				if (Squadron.GetLiveCount() == 0)
				{
					if (Input.GetButtonUp("SquadronNext"))
					{
						Respawn();
					}
					if (Input.GetButtonUp("SquadronPrevious"))
					{
						Respawn();
					}
				}
			}

			if (Input.GetKeyUp(KeyCode.R))
			{
				Respawn();
			}

			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Menus.Current.ToggleQuitMenu();
			}

			// Shortcut to hangar screen
			if (Input.GetKeyUp(KeyCode.H))
			{
				ShipProfileScreen.Current.Fighters.Clear();
				foreach (var member in Squadron.Members)
				{
					ShipProfileScreen.Current.Fighters.Add(member);
				}
				ShipProfileScreen.Current.Populate(Squadron.GetCurrentIndex());
				ShipProfileScreen.Current.Show();
			}

			if (Input.GetKeyUp(KeyCode.Z))
			{
				if (_playVehicleInstance != null)
					_playVehicleInstance.Killable.Die();
			}

			if (Input.GetButtonUp("SquadronNext"))
			{
				CycleSquadron(1);
			}

			if (Input.GetButtonUp("SquadronPrevious"))
			{
				CycleSquadron(-1);
			}

			if (Input.GetKey(KeyCode.M))
			{
				Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.3f, 0.7f);
				//_playVehicleInstance.GetComponent<Killable>().Damage(5f, Vector3.zero, Vector3.forward);
			}
		}

		if (Input.GetKeyUp(KeyCode.Tab))
		{
			Map.Current.Toggle();
		}

		Vehicle leaderVehicle;
		if (Squadron.GetCurrentIndex() == 0)
		{
			leaderVehicle = VehicleInstance;
		}
		else
		{
			_leader = Squadron.GetMember(0);
			leaderVehicle = Squadron.GetMember(0).VehicleInstance;
			if (leaderVehicle != null && _playVehicleInstance != null)
			{
				_leader.IdleDestination = leaderVehicle.transform.position + leaderVehicle.transform.forward * 10f;
				var leaderToPlayer = leaderVehicle.Shiftable.transform.position - _playVehicleInstance.transform.position;
				if (leaderToPlayer.sqrMagnitude > 1000f * 1000f)
					_leader.IdleDestination = _playVehicleInstance.transform.position;
				_leader.IdleUpDestination = leaderVehicle.transform.up;
			}
		}

		if (leaderVehicle != null)
		{
			for (var i = 1; i < Squadron.GetLiveCount(); i++)
			{
				var formationOffset = Formations.GetArrowOffset(i, 10f);
				var formationDestination = leaderVehicle.transform.position + leaderVehicle.transform.rotation * formationOffset;
				Squadron.GetMember(i).IdleDestination = formationDestination;
				Squadron.GetMember(i).IdleUpDestination = leaderVehicle.transform.up;
				//Debug.DrawLine(formationDestination, formationDestination + Vector3.up*100f, Color.white);
			}
		}

		if (_playVehicleInstance != null)
		{
			if (_threatCheckCooldown >= 0f)
			{
				_threatCheckCooldown -= Time.deltaTime;
				if (_threatCheckCooldown < 0f)
				{
					// TODO: Use non allocated Physics.OverlapSphereNonAlloc
					//var count = Physics.OverlapSphereNonAlloc(_playVehicleInstance.transform.position, ThreatRadius, colliders, _detectableMask);
					//for (var i=0; i<count;i++)
					//{
					//	var 
					//}

					var detected = Physics.OverlapSphere(_playVehicleInstance.transform.position, ThreatRadius, _detectableMask);
					_threatCount = detected.Count(d => d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>() != null && d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>().Team == Targeting.GetEnemyTeam(Team));
					_threatCheckCooldown = 1f;
				}
			}

			if (_threatCount > 0)
			{
				_noThreatCooldown = NoThreatTime;
			}
			else
			{
				if (_noThreatCooldown > 0f)
				{
					_noThreatCooldown -= Time.deltaTime;
					//Debug.Log("NO THREAT: " + _noThreatCooldown);
				}
			}

			if (Squadron.GetLiveCount() < Squadron.GetMemberCount())
			{
				//Debug.Log("REPLENISH CHECK!");
				if (_noThreatCooldown < 0f)
				{
					//Debug.Log("REPLENISH!!!");
					for (var i = 0; i < Squadron.GetMemberCount(); i++)
					{
						if (i != Squadron.GetCurrentIndex())
						{
							if (Squadron.GetMember(i).VehicleInstance == null)
							{
								_spawnPos = Universe.Current.GetUniversePosition(Utility.GetRandomDirection(-Universe.Current.ViewPort.transform.forward, 80f) * 2000f);
								Squadron.SpawnSquadronVehicle(Squadron.GetMember(i), _spawnPos, Quaternion.identity);
							}
						}
					}
					HeadsUpDisplay.Current.RefreshSquadronIcons();
				}
			}
		}

		if (_playVehicleInstance != null)
		{
			Debug.DrawLine(_playVehicleInstance.transform.position, _playVehicleInstance.transform.position + Vector3.up * 100f, Color.magenta);
		}
	}

	private void Respawn()
	{
		if (_playVehicleInstance != null)
			_playVehicleInstance.Killable.Die();
		Debug.Log("RESPAWN");
		var respawnAt = SpawnManager.FindNearest(_lastDeathUniversePosition);
		respawnAt.Spawn();
		_isAllowRespawn = false;
		HeadsUpDisplay.Current.RefreshSquadronIcons();
	}

	private string _curCallSlign;

	public string GetCallSign()
	{
		return _curCallSlign;
	}

	private void CycleSquadron(int dir)
	{
		var oldSquadronIndex = Squadron.GetCurrentIndex();
		var cycleResult = Squadron.CycleSquadronIndex(dir);

		Debug.LogFormat("CYCLE {0} => {1}", oldSquadronIndex, Squadron.GetCurrentIndex());

		if (cycleResult > -1)
		{
			if (Squadron.GetCurrentMember() != null)
			{
				// Set previous controlled vehicle to NPC control
				var oldMember = Squadron.GetMember(oldSquadronIndex);
				if (_playVehicleInstance != null && oldMember != null)
				{
					if (_playVehicleInstance.PrimaryWeaponInstance != null)
						_playVehicleInstance.PrimaryWeaponInstance.ClearTargetLock();
					if (_playVehicleInstance.SecondaryWeaponInstance != null)
						_playVehicleInstance.SecondaryWeaponInstance.ClearTargetLock();
					_playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Default");
					_playVehicleInstance.MeshTransform.gameObject.layer = LayerMask.NameToLayer("Default");

					oldMember.SetVehicleInstance(_playVehicleInstance);
					oldMember.enabled = true;
					oldMember.VehicleInstance.GetComponent<SquadronTracker>().IsDisabled = false;

					oldMember.VehicleInstance.GetComponent<MapPin>().SetPinState(MapPin.MapPinState.Inactive);

					oldMember.VehicleInstance.Killable.OnDamage -= PlayerController_OnDamage;
					oldMember.VehicleInstance.Killable.OnDie -= PlayerController_OnDie;
					oldMember.VehicleInstance.GetComponent<ShieldRegenerator>().OnRegenerate -= PlayerController_OnRegenerate;
				}

				// Disable next vehicle NPC control and apply PlayerController
				var curMember = Squadron.GetCurrentMember();
				if (curMember.VehicleInstance != null)
				{
					var profile = curMember.GetComponent<ShipProfile>();
					HeadsUpDisplay.Current.ShowSquadronPrompt(profile.CallSign);

					_playVehicleInstance = curMember.VehicleInstance;
					_playVehicleInstance.GetComponent<SquadronTracker>().IsDisabled = true;
					_playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");
					_playVehicleInstance.MeshTransform.gameObject.layer = LayerMask.NameToLayer("Player");

					_playVehicleInstance.GetComponent<MapPin>().SetPinState(MapPin.MapPinState.Active);

					_playVehicleInstance.Killable.OnDamage += PlayerController_OnDamage;
					_playVehicleInstance.Killable.OnDie += PlayerController_OnDie;
					_playVehicleInstance.GetComponent<ShieldRegenerator>().OnRegenerate += PlayerController_OnRegenerate;
					_playVehicleInstance.Controller = gameObject;

					_curCallSlign = profile.CallSign;
					curMember.enabled = false;

					Universe.Current.WarpTo(_playVehicleInstance.Shiftable);

					var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
					cam.Target = _playVehicleInstance;
					cam.Reset();
				}
				if (OnChangeSquadronMember != null)
					OnChangeSquadronMember(oldMember.gameObject, gameObject);
			}
			HeadsUpDisplay.Current.RefreshSquadronIcon(Squadron.GetCurrentIndex());
			HeadsUpDisplay.Current.RefreshSquadronIcon(oldSquadronIndex);
		}
		else
		{
			Debug.Log("ALL DEAD!");
		}
	}

	private void PlayerController_OnRegenerate()
	{
		HeadsUpDisplay.Current.RefreshSquadronIcon(0);
	}

	private void PlayerController_OnDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
	{
		HeadsUpDisplay.Current.Hit();
		Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.3f, 0.7f, 0.1f);
		HeadsUpDisplay.Current.RefreshSquadronIcon(0);
        if (VehicleInstance.Killable.Shield > 0f)
        {
            HeadsUpDisplay.Current.TriggerShieldHit();
        }
        else
        {
            HeadsUpDisplay.Current.TriggerHealthHit();
        }
	}

	private void PlayerController_OnDie(Killable sender)
	{
		_noThreatCooldown = NoThreatTime;
		_deathCooldown = DeathOptionTime;
		_lastDeathUniversePosition = new UniversePosition(_playVehicleInstance.Shiftable.UniversePosition.CellIndex, _playVehicleInstance.Shiftable.UniversePosition.CellLocalPosition);
		Debug.Log("PLAYER VEHICLE DESTROYED AT: " + _lastDeathUniversePosition.CellIndex);
		HeadsUpDisplay.Current.RefreshSquadronIcon(0);
	}

	public void ResetThreatCooldown()
	{
		_noThreatCooldown = NoThreatTime;
	}

	public bool InPlayerActiveCells(CellIndex checkCell)
	{
		var playerCellIndex = _playVehicleInstance.Shiftable.UniverseCellIndex;
		for (var x = -1; x < 2; x++)
		{
			for (var y = -1; y < 2; y++)
			{
				for (var z = -1; z < 2; z++)
				{
					if (checkCell.IsEqualTo(playerCellIndex + new CellIndex(x, y, z)))
						return true;
				}
			}
		}
		return false;
	}

	public static PlayerController Current
	{
		get { return _current; }
	}

	public Vehicle VehicleInstance
	{
		get { return _playVehicleInstance; }
	}

	private CellIndex _cellIndex;
	//private void OnGUI()
	//{
	//	//GUI.Label(new Rect(Screen.width - 100f, Screen.height - 100f, 100f, 25), string.Format("{0:f2} m/s", VehicleInstance.GetVelocity().magnitude));
	//	if (_playVehicleInstance == null)
	//	{
	//		if (Squadron.GetLiveCount() == 0)
	//		{
	//			if (_isAllowRespawn)
	//				GUI.Label(new Rect(Screen.width / 2f - 100f, Screen.height / 2f + 50f, 200f, 25f), "Press 'Fire' to respawn.", new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
	//		}
	//		else
	//		{
	//			if (_isAllowRespawn)
	//				GUI.Label(new Rect(Screen.width / 2f - 100f, Screen.height / 2f + 50f, 200f, 25f), "Press 'Next' or 'Previous' to select another Squadron Member.", new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
	//		}
	//	}
	//	_cellIndex = Universe.Current.ViewPort.Shiftable.UniverseCellIndex;
	//	GUI.Label(new Rect(30f, 120f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", _cellIndex.X, _cellIndex.Y, _cellIndex.Z));
	//	GUI.Label(new Rect(30f, 150f, 100f, 25f), string.Format("SQUADRON: {0:f0}/{1:f0}", Squadron.GetLiveCount(), Squadron.GetMemberCount()));
	//	GUI.Label(new Rect(30f, 180f, 100f, 25f), string.Format("THREATS: {0}", _threatCount));
	//	//GUI.Label(new Rect(30f, 180f, 200f, 25f), string.Format("LOCK: {0} ({1:f2})", lockingTarget != null ? lockingTarget.name : string.Empty, lockingTime));
	//	//GUI.Label(new Rect(30f, 210f, 200f, 25f), string.Format("LOCKED: {0}", VehicleInstance.SecondaryWeaponInstance.GetLockedOnTarget() != null ? VehicleInstance.SecondaryWeaponInstance.GetLockedOnTarget().name : string.Empty));
	//	GUI.Label(new Rect(30f, 240f, 200f, 25f), string.Format("AIM DIST: {0:f2}", _aimDistance));
	//	GUI.Label(new Rect(30f, 270f, 200f, 25f), string.Format("SPACE JUNK: {0:f0}", SpaceJunkCount), new GUIStyle { normal = { textColor = new Color(0f, 0.8f, 0.56f, 1f) } });

	//	GUI.Label(new Rect(30f, 300f, 200f, 25f), string.Format("POWER NODES: {0:f0}", PowerNodeCount), new GUIStyle { normal = { textColor = new Color(0.8f, 0.56f, 1f, 1f) } });
	//	GUI.Label(new Rect(30f, 330f, 200f, 25f), string.Format("SHIFTABLES: {0}", Universe.Current.ShiftableItems.Count));
	//}

	private void OnDrawGizmos()
	{
		if (_playVehicleInstance != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(GetAimAt(), 1f);
		}
	}
}