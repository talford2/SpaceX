using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Vehicle VehiclePrefab;
	public Team Team;

	private Vehicle _playVehicleInstance;
	private Fighter _playerNpc;

	public List<Fighter> Squadron;
	private int _curSquadronIndex = 0;

    private bool _controlEnabled;

	public bool InvertY = false;
	public bool HideMouse = false;

	public Shiftable RespawnPosition;

    public GameObject PlayerPinPrefab;
    public GameObject SquadronPinPrefab;

	[Header("Squadron Trackers")]
	public Texture2D ArrowCursorImage;
	public Texture2D TrackerCurosrImage;
	public Texture2D FarTrackerCursorImage;
	public Texture2D VeryFarTrackerCursorImage;
    public Texture2D LockingTrackerCursorImage;
    public Texture2D LockedTrackerCursorImage;

	[Header("Aiming")]
	public float AimSensitivity = 10f;
    public float MouseMoveClamp = 0.02f; 

	public float DefaultAimDistance = 200f;
	public float MinAimDistance = 10f;
	public float MaxAimDistance = 1000f;

    private float aimDistance;
	private float screenAspect;
	private int squadronLiveCount;
	private float replenishInterval = 2f;
	private float replenishSquadronCooldown;

    private UniversePosition lastDeathUniversePosition;

	private void Awake()
	{
		_current = this;
		Cursor.visible = !HideMouse;
		if (HideMouse)
			Cursor.lockState = CursorLockMode.Locked;

		screenAspect = (float)Screen.height / (float)Screen.width;

		_playerNpc = gameObject.AddComponent<Fighter>();
		_playerNpc.Team = Team;
		_playerNpc.IsSquadronMember = true;
		_playerNpc.VehiclePrefab = VehiclePrefab;
		_playerNpc.enabled = false;
		SpawnVehicle(VehiclePrefab, Universe.Current.PlayerSpawnPosition);
		_playerNpc.SetVehicleInstance(_playVehicleInstance);
		_playerNpc.IsFollowIdleDestination = true;
		Squadron.Insert(0, _playerNpc);

        SetControlEnabled(true);
	}

	private void Start()
	{
		//foreach (var member in Squadron)
		for (var i = 0; i < Squadron.Count; i++)
		{
			var member = Squadron[i];
			if (member != _playerNpc)
			{
				var univPos = _playVehicleInstance.Shiftable.UniversePosition;
				univPos.CellLocalPosition += Formations.GetArrowOffset(i, 10f);
				member.IsSquadronMember = true;
				SpawnSquadronVehicle(member, univPos);
			}
		}
	}

	private void SpawnSquadronVehicle(Fighter member, UniversePosition position)
	{
		member.SpawnVehicle(member.VehiclePrefab, position);
		var memberTracker = member.VehicleInstance.GetComponent<Tracker>();
		memberTracker.ArrowCursorImage = ArrowCursorImage;
		memberTracker.TrackerCurosrImage = TrackerCurosrImage;
		memberTracker.FarTrackerCursorImage = FarTrackerCursorImage;
		memberTracker.VeryFarTrackerCursorImage = VeryFarTrackerCursorImage;
	    memberTracker.LockingCursorImage = LockingTrackerCursorImage;
	    memberTracker.LockedCursorImage = LockedTrackerCursorImage;
		member.IsFollowIdleDestination = true;
	    var mapPin = member.VehicleInstance.gameObject.AddComponent<MapPin>();
	    mapPin.ActivePin = SquadronPinPrefab;
	    mapPin.InactivePin = SquadronPinPrefab;
	    var squadronHealthRegenerator = member.VehicleInstance.gameObject.AddComponent<HealthRegenerator>();
	    squadronHealthRegenerator.RegenerationDelay = 5f;
	    squadronHealthRegenerator.RegenerationRate = 5f;
	    member.enabled = true;
	}

	public void SpawnVehicle(Vehicle vehiclePrefab, Shiftable spawner)
	{
		SpawnVehicle(vehiclePrefab, spawner.UniversePosition, spawner.transform.rotation);
	}

    public void SpawnVehicle(Vehicle vehiclePrefab, UniversePosition universePosition, Quaternion rotation)
    {
        _playVehicleInstance = ((GameObject)Instantiate(vehiclePrefab.gameObject, universePosition.CellLocalPosition, rotation)).GetComponent<Vehicle>();
        _playVehicleInstance.Shiftable.SetShiftPosition(universePosition);
        //Destroy(_playVehicleInstance.GetComponent<Tracker>());

        var playerTracker = _playVehicleInstance.GetComponent<Tracker>();
        playerTracker.ArrowCursorImage = ArrowCursorImage;
        playerTracker.TrackerCurosrImage = TrackerCurosrImage;
        playerTracker.FarTrackerCursorImage = FarTrackerCursorImage;
        playerTracker.VeryFarTrackerCursorImage = VeryFarTrackerCursorImage;
        playerTracker.IsDisabled = true;

        _playVehicleInstance.GetComponent<Targetable>().Team = Team;
        _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");

        _playVehicleInstance.GetComponent<Killable>().OnDamage += PlayerController_OnDamage;
        _playVehicleInstance.GetComponent<Killable>().OnDie += PlayerController_OnDie;

        var healthRegenerator = _playVehicleInstance.gameObject.AddComponent<HealthRegenerator>();
        healthRegenerator.RegenerationDelay = 5f;
        healthRegenerator.RegenerationRate = 5f;
    }

    public int GetSquadronSelectedIndex()
    {
        return _curSquadronIndex;
    }

	private void PlayerController_OnDamage(Vector3 position, Vector3 normal)
	{
		HeadsUpDisplay.Current.Hit();
        Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(25f, 0.1f, 0.05f);
	}

	private Vector3 GetAimAt()
	{
		var mouseRay = Universe.Current.ViewPort.AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit aimHit;
	    aimDistance = Mathf.Lerp(aimDistance, DefaultAimDistance, Time.deltaTime);
		var aimAtPosition = mouseRay.GetPoint(DefaultAimDistance);

        // Fancy System.
	    var viewPortPos = Universe.Current.ViewPort.transform.position;
        var viewPortForward = Universe.Current.ViewPort.transform.forward;
        var dotViewPort = Vector3.Dot(VehicleInstance.PrimaryWeaponInstance.GetShootPointCentre() - viewPortPos, viewPortForward);
        var guessTarget = Targeting.FindFacingAngleAny(viewPortPos + dotViewPort * viewPortForward, viewPortForward, MaxAimDistance, 5f);
	    if (guessTarget != null)
	    {
	        var toGuessTarget = guessTarget.position - viewPortPos;
            aimDistance = Mathf.Clamp(toGuessTarget.magnitude, MinAimDistance, MaxAimDistance);
            return mouseRay.GetPoint(aimDistance);
	    }

	    if (Physics.Raycast(mouseRay, out aimHit, MaxAimDistance, ~LayerMask.GetMask("Player", "Detectable")))
	    {
	        aimDistance = Mathf.Clamp(aimHit.distance, MinAimDistance, MaxAimDistance);
            aimAtPosition = mouseRay.GetPoint(aimDistance);
	    }
	    return aimAtPosition;
	}

    public void SetControlEnabled(bool value)
    {
        _controlEnabled = value;
    }

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
	            var mouseHorizontal = AimSensitivity*Input.GetAxis("MouseHorizontal")/Screen.width;
	            var mouseVertical = AimSensitivity*screenAspect*Input.GetAxis("MouseVertical")/Screen.height;

	            var controllerHorizontal = AimSensitivity*Input.GetAxis("Horizontal")/Screen.width;
	            var controllerVertical = AimSensitivity*screenAspect*Input.GetAxis("Vertical")/Screen.height;

	            var pitchYaw = Vector2.ClampMagnitude(new Vector2(controllerVertical + mouseVertical, controllerHorizontal + mouseHorizontal), MouseMoveClamp);

	            if (InvertY)
	            {
	                _playVehicleInstance.PitchThotttle = pitchYaw.x*-1;
	            }
	            else
	            {
	                _playVehicleInstance.PitchThotttle = pitchYaw.x;
	            }
	            _playVehicleInstance.YawThrottle = pitchYaw.y;
	            _playVehicleInstance.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
	            _playVehicleInstance.PrimaryWeaponInstance.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;
	            _playVehicleInstance.SecondaryWeaponInstance.IsTriggered = (Input.GetAxis("AltFireTrigger") + Input.GetAxis("MouseAltFireTrigger")) > 0;

	            _playVehicleInstance.SetAimAt(GetAimAt());

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
	        }

	        if (Input.GetKeyUp(KeyCode.R))
	        {
	            if (_playVehicleInstance != null)
	                _playVehicleInstance.GetComponent<Killable>().Die();
	            Debug.Log("RESPAWN");
                
	            var respawnAt = SpawnManager.FindNearest(lastDeathUniversePosition);
                respawnAt.Spawn();
	            /*
	            Universe.Current.WarpTo(RespawnPosition);
	            _curSquadronIndex = 0;
	            SpawnVehicle(VehiclePrefab, RespawnPosition);

	            var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
	            cam.Target = _playVehicleInstance;
	            cam.Reset();
                */
	        }

	        if (Input.GetKeyUp(KeyCode.Escape))
	        {
	            Menus.Current.ToggleQuitMenu();
	        }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                if (_playVehicleInstance != null)
                    _playVehicleInstance.GetComponent<Killable>().Die();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                CycleSquadron();
            }

            if (Input.GetKeyUp(KeyCode.M))
            {
                Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(25f, 1f, 0.05f);
                //_playVehicleInstance.GetComponent<Killable>().Damage(5f, Vector3.zero, Vector3.forward);
            }
	    }

	    if (Input.GetKeyUp(KeyCode.Tab))
	    {
	        Map.Current.Toggle();
	    }

		// Update squadron
		squadronLiveCount = Squadron.Count(s => s.VehicleInstance != null);

        Vehicle leaderVehicle;
        if (_curSquadronIndex == 0)
        {
            leaderVehicle = VehicleInstance;
        }
        else
        {
            leaderVehicle = Squadron[0].VehicleInstance;
            if (leaderVehicle != null)
            {
                Squadron[0].IdleDestination = leaderVehicle.transform.position + leaderVehicle.transform.forward * 10f;
                var leaderToPlayer = leaderVehicle.Shiftable.transform.position - _playVehicleInstance.transform.position;
                if (leaderToPlayer.sqrMagnitude > 1000f*1000f)
                    Squadron[0].IdleDestination = _playVehicleInstance.transform.position;
                Squadron[0].IdleUpDestination = leaderVehicle.transform.up;
            }
        }

	    if (leaderVehicle != null)
	    {
	        for (var i = 1; i < squadronLiveCount; i++)
	        {
	            var formationOffset = Formations.GetArrowOffset(i, 10f);
	            var formationDestination = leaderVehicle.transform.position + leaderVehicle.transform.rotation*formationOffset;
	            Squadron[i].IdleDestination = formationDestination;
	            Squadron[i].IdleUpDestination = leaderVehicle.transform.up;
	            //Debug.DrawLine(formationDestination, formationDestination + Vector3.up*100f, Color.white);
	        }
	    }

	    if (_playVehicleInstance != null)
		{
			if (squadronLiveCount < Squadron.Count)
			{
				if (replenishSquadronCooldown >= 0f)
				{
					replenishSquadronCooldown -= Time.deltaTime;
					if (replenishSquadronCooldown < 0)
					{
						//Debug.Log("REPLENISH CHECK!");
						var detected = Physics.OverlapSphere(_playVehicleInstance.transform.position, 2000f, LayerMask.GetMask("Detectable"));
						if (detected.Any(d => d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>().Team == Targeting.GetEnemyTeam(Team)))
						{
						}
						else
						{
							//Debug.Log("REPLENISH!!!");
							for (var i = 0; i < Squadron.Count; i++)
							{
								if (i != _curSquadronIndex)
								{
									if (Squadron[i].VehicleInstance == null)
									{
										var spawnPos = Universe.Current.GetUniversePosition(Utility.GetRandomDirection(-Universe.Current.ViewPort.transform.forward, 80f) * 2000f);
										SpawnSquadronVehicle(Squadron[i], spawnPos);
									}
								}
							}
						}
						replenishSquadronCooldown = replenishInterval;
					}
				}
			}
		}

		if (_playVehicleInstance != null)
		{
			Debug.DrawLine(_playVehicleInstance.transform.position, _playVehicleInstance.transform.position + Vector3.up * 100f, Color.magenta);
		}
	}

	private int CycleSquadronIndex()
	{
		if (Squadron.Any(s => s.VehicleInstance != null))
		{
			_curSquadronIndex++;
			if (_curSquadronIndex >= Squadron.Count)
				_curSquadronIndex = 0;
			if (Squadron[_curSquadronIndex].VehicleInstance == null)
				return CycleSquadronIndex();
			return _curSquadronIndex;
		}
		return -1;
	}

    private void CycleSquadron()
    {
        var oldSquadronIndex = _curSquadronIndex + 0;
        var cycleResult = CycleSquadronIndex();
        if (cycleResult > -1)
            _curSquadronIndex = cycleResult;

        Debug.LogFormat("CYCLE {0} => {1}", oldSquadronIndex, _curSquadronIndex);

        if (cycleResult > -1)
        {
            if (Squadron[_curSquadronIndex] != null)
            {
                // Set previous controlled vehicle to NPC control
                if (_playVehicleInstance != null && Squadron[oldSquadronIndex] != null)
                {
                    _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Default");
                    //_playVehicleInstance.GetComponent<Killable>().OnDie -= OnVehicleDestroyed;
                    Squadron[oldSquadronIndex].SetVehicleInstance(_playVehicleInstance);
                    Squadron[oldSquadronIndex].enabled = true;
                    Squadron[oldSquadronIndex].VehicleInstance.GetComponent<Tracker>().IsDisabled = false;
                    Squadron[oldSquadronIndex].VehicleInstance.GetComponent<Killable>().OnDamage -= PlayerController_OnDamage;
                    Squadron[oldSquadronIndex].VehicleInstance.GetComponent<Killable>().OnDie -= PlayerController_OnDie;
                }

                // Disable next vehicle NPC control and apply PlayerController
                if (Squadron[_curSquadronIndex].VehicleInstance != null)
                {
                    HeadsUpDisplay.Current.ShowSquadronPrompt(string.Format("{0:f0}", _curSquadronIndex));

                    _playVehicleInstance = Squadron[_curSquadronIndex].VehicleInstance;
                    _playVehicleInstance.GetComponent<Tracker>().IsDisabled = true;
                    _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");
                    //_playVehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;
                    _playVehicleInstance.GetComponent<Killable>().OnDamage += PlayerController_OnDamage;
                    _playVehicleInstance.GetComponent<Killable>().OnDie += PlayerController_OnDie;

                    Squadron[_curSquadronIndex].enabled = false;

                    Universe.Current.WarpTo(_playVehicleInstance.Shiftable);

                    var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
                    cam.Target = _playVehicleInstance;
                    cam.Reset();
                }
            }
        }
        else
        {
            Debug.Log("ALL DEAD!");
        }
    }

    private void PlayerController_OnDie(Killable sender)
    {
        lastDeathUniversePosition = new UniversePosition(_playVehicleInstance.Shiftable.UniversePosition.CellIndex, _playVehicleInstance.Shiftable.UniversePosition.CellLocalPosition);
        Debug.Log("PLAYER VEHICLE DESTROYED AT: " + lastDeathUniversePosition.CellIndex);
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

	private static PlayerController _current;

	public static PlayerController Current
	{
		get { return _current; }
	}

	public Vehicle VehicleInstance
	{
		get { return _playVehicleInstance; }
	}

	private void OnGUI()
	{
		//GUI.Label(new Rect(Screen.width - 100f, Screen.height - 100f, 100f, 25), string.Format("{0:f2} m/s", VehicleInstance.GetVelocity().magnitude));
		if (_playVehicleInstance == null)
		{
			if (squadronLiveCount == 0)
			{
				GUI.Label(new Rect(Screen.width / 2f - 100f, Screen.height / 2f + 50f, 200f, 25f), "Press 'R' to respawn.", new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
			}
			else
			{
				GUI.Label(new Rect(Screen.width / 2f - 100f, Screen.height / 2f + 50f, 200f, 25f), "Press 'E' to select next Squadron Member.", new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
			}
		}
        var cellIndex = Universe.Current.ViewPort.Shiftable.UniverseCellIndex;
        GUI.Label(new Rect(30f, 120f, 200f, 20f), string.Format("CELL ({0}, {1}, {2})", cellIndex.X, cellIndex.Y, cellIndex.Z));
		GUI.Label(new Rect(30f, 150f, 100f, 25f), string.Format("SQUADRON: {0:f0}/{1:f0}", squadronLiveCount, Squadron.Count));
		//GUI.Label(new Rect(30f, 180f, 200f, 25f), string.Format("LOCK: {0} ({1:f2})", lockingTarget != null ? lockingTarget.name : string.Empty, lockingTime));
        GUI.Label(new Rect(30f, 210f, 200f, 25f), string.Format("LOCKED: {0}", VehicleInstance.SecondaryWeaponInstance.GetLockedOnTarget() != null ? VehicleInstance.SecondaryWeaponInstance.GetLockedOnTarget().name : string.Empty));
        GUI.Label(new Rect(30f, 240f, 200f, 25f), string.Format("AIM DIST: {0:f2}", aimDistance));
	}

	private void OnDrawGizmos()
	{
		if (_playVehicleInstance != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(GetAimAt(), 1f);
		}
	}
}