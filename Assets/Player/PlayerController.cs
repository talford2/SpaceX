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
	private int _curVehicleIndex = 0;

	public bool InvertY = false;
	public bool HideMouse = false;

	public Shiftable RespawnPosition;
    [Header("Squadron Trackers")]
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCurosrImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;

	[Header("Aiming")]
	public float AimSensitivity = 10f;

    public float DefaultAimDistance = 200f;
	public float MinAimDistance = 10f;
	public float MaxAimDistance = 1000f;

	private float screenAspect;
    private int squadronLiveCount;
    private float replenishInterval = 2f;
    private float replenishSquadronCooldown;

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
	    _playerNpc.enabled = false;
        SpawnVehicle(VehiclePrefab, Universe.Current.PlayerSpawnPosition);
        _playerNpc.SetVehicleInstance(_playVehicleInstance);
	    _playerNpc.IsFollowIdleDestination = true;
        Squadron.Insert(0, _playerNpc);
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
                /*
                member.SpawnVehicle(member.VehiclePrefab, univPos);
                var memberTracker = member.VehicleInstance.GetComponent<Tracker>();
                memberTracker.ArrowCursorImage = ArrowCursorImage;
                memberTracker.TrackerCurosrImage = TrackerCurosrImage;
                memberTracker.FarTrackerCursorImage = FarTrackerCursorImage;
                memberTracker.VeryFarTrackerCursorImage = VeryFarTrackerCursorImage;
                member.IsFollowIdleDestination = true;
                */
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
        member.IsFollowIdleDestination = true;
    }

    private void SpawnVehicle(Vehicle vehiclePrefab, Shiftable spawner)
	{
		_playVehicleInstance = ((GameObject)Instantiate(vehiclePrefab.gameObject, spawner.CellLocalPosition, spawner.transform.rotation)).GetComponent<Vehicle>();
		_playVehicleInstance.Shiftable.SetShiftPosition(spawner.UniversePosition);
		//Destroy(_playVehicleInstance.GetComponent<Tracker>());

	    var playerTracker = _playVehicleInstance.GetComponent<Tracker>();
	    playerTracker.ArrowCursorImage = ArrowCursorImage;
	    playerTracker.TrackerCurosrImage = TrackerCurosrImage;
        playerTracker.FarTrackerCursorImage = FarTrackerCursorImage;
        playerTracker.VeryFarTrackerCursorImage = VeryFarTrackerCursorImage;
	    playerTracker.IsDisabled = true;

	    _playVehicleInstance.GetComponent<Targetable>().Team = Team;
		_playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");
	    //_playVehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;
	}

	private Vector3 GetAimAt()
	{
		var mouseRay = Universe.Current.ViewPort.AttachedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit aimHit;
        var aimAtPosition = mouseRay.GetPoint(DefaultAimDistance);
		if (Physics.Raycast(mouseRay, out aimHit, MaxAimDistance, ~LayerMask.GetMask("Player", "Detectable")))
		{
			if (aimHit.distance > MinAimDistance)
			{
				aimAtPosition = aimHit.point;
			}
		}
		return aimAtPosition;
	}

    private Transform lockingTarget;
    private Transform lastLockingTarget;
    private Transform lockedTarget;
    private float lockingTime;
    private float lockTime = 1.5f;
    private bool isLocked;

    private void TagetLocking()
    {
        if (!isLocked)
        {
            lockingTarget = null;
            if (Input.GetMouseButton(1))
            {
                lockingTarget = Targeting.FindFacingAngle(Targeting.GetEnemyTeam(Team), VehicleInstance.transform.position, VehicleInstance.transform.forward, 1000f);
                if (lastLockingTarget == null)
                    lastLockingTarget = lockingTarget;
            }
            else
            {
                lastLockingTarget = null;
            }

            if (lastLockingTarget != null && lastLockingTarget == lockingTarget)
            {
                lockingTime += Time.deltaTime;
                if (lockingTime > lockTime)
                {
                    lockedTarget = lockingTarget;
                    isLocked = true;
                }
            }
            else
            {
                lockingTime = 0f;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                Debug.Log("RELEASE!");
                isLocked = false;
                lastLockingTarget = null;
                lockedTarget = null;
            }
        }
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Debug.Break();
		}
	    if (_playVehicleInstance != null)
	    {
	        var mouseClamp = 0.003f;
	        var mouseHorizontal = AimSensitivity*Mathf.Clamp(Input.GetAxis("MouseHorizontal")/Screen.width, -mouseClamp, mouseClamp);
	        var mouseVertical = AimSensitivity*Mathf.Clamp(screenAspect*Input.GetAxis("MouseVertical")/Screen.height, -mouseClamp, mouseClamp);

	        var controllerHorizontal = AimSensitivity*Input.GetAxis("Horizontal")/Screen.width;
	        var controllerVertical = AimSensitivity*screenAspect*Input.GetAxis("Vertical")/Screen.height;

	        if (InvertY)
	        {
	            _playVehicleInstance.PitchThotttle = (controllerVertical + mouseVertical)*-1;
	        }
	        else
	        {
	            _playVehicleInstance.PitchThotttle = controllerVertical + mouseVertical;
	        }
	        _playVehicleInstance.YawThrottle = controllerHorizontal + mouseHorizontal;
	        _playVehicleInstance.RollThrottle = Input.GetAxis("Roll") + Input.GetAxis("KeyboardRoll");
	        _playVehicleInstance.CurrentWeapon.IsTriggered = (Input.GetAxis("FireTrigger") + Input.GetAxis("MouseFireTrigger")) > 0;

	        TagetLocking();

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
	        Universe.Current.WarpTo(RespawnPosition);
	        _curVehicleIndex = 0;
	        SpawnVehicle(VehiclePrefab, RespawnPosition);

	        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
	        cam.Target = _playVehicleInstance;
	        cam.Reset();
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

        // Update squadron
        squadronLiveCount = Squadron.Count(s => s.VehicleInstance != null);

	    for (var i = 0; i < squadronLiveCount; i++)
	    {
	        if (_playVehicleInstance != null)
	        {
	            var formationOffset = Formations.GetArrowOffset(i, 10f);
	            //Debug.Log(Squadron[i].name + " FORMATION OFFSET " + i + " : " + formationOffset);
	            var formationDestination = _playVehicleInstance.transform.position + _playVehicleInstance.transform.rotation*formationOffset;
	            Squadron[i].IdleDestination = formationDestination;
	            if (i > 0)
	                Debug.DrawLine(formationDestination, formationDestination + Vector3.up*100f, Color.white);
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
                        Debug.Log("REPLENISH CHECK!");
	                    var detected = Physics.OverlapSphere(_playVehicleInstance.transform.position, 2000f, LayerMask.GetMask("Detectable"));
	                    if (detected.Any(d => d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>().Team == Targeting.GetEnemyTeam(Team)))
	                    {

	                    }
	                    else
	                    {
	                        Debug.Log("REPLENISH!!!");
	                        for (var i = 0; i < Squadron.Count; i++)
	                        {
	                            if (Squadron[i].VehicleInstance == null)
	                            {
                                    var spawnPos = Universe.Current.GetUniversePosition(Utility.GetRandomDirection(-Universe.Current.ViewPort.transform.forward, 80f) * 2000f);
                                    SpawnSquadronVehicle(Squadron[i], spawnPos);
	                                //Squadron[i].SpawnVehicle(Squadron[i].VehiclePrefab, spawnPos);
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
            _curVehicleIndex++;
            if (_curVehicleIndex >= Squadron.Count)
                _curVehicleIndex = 0;
            if (Squadron[_curVehicleIndex].VehicleInstance == null)
                return CycleSquadronIndex();
            return _curVehicleIndex;
        }
        return -1;
    }

    private void CycleSquadron()
    {
        var oldSquadronIndex = _curVehicleIndex + 0;
        _curVehicleIndex = CycleSquadronIndex();

        Debug.LogFormat("CYCLE {0} => {1}", oldSquadronIndex, _curVehicleIndex);

        if (_curVehicleIndex > -1)
        {
            if (Squadron[_curVehicleIndex] != null)
            {
                // Set previous controlled vehicle to NPC control
                if (_playVehicleInstance != null && Squadron[oldSquadronIndex] != null)
                {
                    _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Default");
                    //_playVehicleInstance.GetComponent<Killable>().OnDie -= OnVehicleDestroyed;
                    Squadron[oldSquadronIndex].SetVehicleInstance(_playVehicleInstance);
                    Squadron[oldSquadronIndex].enabled = true;
                    Squadron[oldSquadronIndex].VehicleInstance.GetComponent<Tracker>().IsDisabled = false;
                }

                // Disable next vehicle NPC control and apply PlayerController
                if (Squadron[_curVehicleIndex].VehicleInstance != null)
                {
                    HeadsUpDisplay.Current.ShowSquadronPrompt(string.Format("{0:f0}", _curVehicleIndex));

                    _playVehicleInstance = Squadron[_curVehicleIndex].VehicleInstance;
                    _playVehicleInstance.GetComponent<Tracker>().IsDisabled = true;
                    _playVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");
                    //_playVehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;

                    Squadron[_curVehicleIndex].enabled = false;

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

    /*
    private void OnVehicleDestroyed(Killable sender)
    {
        Debug.Log("PLAYER VEHICLE DESTROYED");
    }
    */

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
                GUI.Label(new Rect(Screen.width/2f - 100f, Screen.height/2f + 50f, 200f, 25f), "Press 'R' to respawn.", new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}});
            }
            else
            {
                GUI.Label(new Rect(Screen.width/2f - 100f, Screen.height/2f + 50f, 200f, 25f), "Press 'E' to select next Squadron Member.", new GUIStyle {alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}});
            }
        }
        GUI.Label(new Rect(50f, 80f, 100f, 25f), string.Format("Squadron: {0:f0}", squadronLiveCount));
        GUI.Label(new Rect(50f, 110f, 200f, 25f), string.Format("LOCK: {0} ({1:f2})", lockingTarget != null ? lockingTarget.name : string.Empty, lockingTime));
        GUI.Label(new Rect(50f, 140f, 200f, 25f), string.Format("LOCKED: {0}", lockedTarget != null ? lockedTarget.name : string.Empty));
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
