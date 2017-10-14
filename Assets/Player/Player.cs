using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vehicle DefaultVehiclePrefab;
    public Team Team;
    public string CallSign;
    public int SpaceJunkCount;
    public int PowerNodeCount;
    public float CollectRadius = 8f;

    private Vehicle _playerVehiclePrefab;
    private Vehicle _playerVehicleInstance;
    private Fighter _playerNpc;
    public Color TrackerColor;

    public AudioClip PlayerHitSound;
    public List<AudioClip> PlayerGetHitSound;
    public List<AudioClip> ShieldGetHitSound;

    public float ThreatRadius = 2000f;
    public PlayerSquadron Squadron;

    private bool _controlEnabled;

    private static Player _current;

    public Shiftable RespawnPosition;

    public Shiftable SpawnPathDestination;
    public float SpawnControlDelay = 2.5f;

    public GameObject PlayerPinPrefab;

    [Header("Other")]
    public float NoThreatTime = 5f;
    public float DeathOptionTime = 1f;

    [Header("UTurn")]
    public SplinePath UTurnPath;
    public float UTurnDuration = 5f;

    private ShipProfile _profile;

    private int _threatCount;
    private float _threatCheckCooldown;
    private float _noThreatCooldown;

    private string _curCallSlign;

    private float _deathCooldown;
    private bool _isAllowRespawn;

    private int _detectableMask;
    private int _collectableMask;
    private UniversePosition _lastDeathUniversePosition;

    private PlayerController _vehicleController;

    public static Player Current
    {
        get { return _current; }
    }

    private void Awake()
    {
        _current = this;

        _vehicleController = GetComponent<PlayerController>();

        if (OptionsFile.Exists())
            _vehicleController.InvertY = OptionsFile.ReadFromFile().InvertYAxis;

        _profile = GetComponent<ShipProfile>();

        _playerNpc = gameObject.AddComponent<Fighter>();
        _playerNpc.Team = Team;
        _playerNpc.IsSquadronMember = true;
        _playerNpc.GetComponent<ShipProfile>().CallSign = CallSign;
        _playerNpc.enabled = false;

        Squadron.AddPlayerToSquadron(_playerNpc);

        _detectableMask = LayerMask.GetMask("Detectable");
        _collectableMask = LayerMask.GetMask("Collectible");
    }

    private void Start()
    {
        if (PlayerFile.Exists())
        {
            Load();
        }
        else
        {
            _playerVehiclePrefab = DefaultVehiclePrefab;
            SpawnVehicle(_playerVehiclePrefab, Universe.Current.PlayerSpawnPosition);
            BuildFile().WriteToFile();
        }

        _playerNpc.VehiclePrefab = _playerVehiclePrefab;
        _playerNpc.SetVehicleInstance(_playerVehicleInstance);
        _playerNpc.IsFollowIdleDestination = true;

        Squadron.Initialize();

        HeadsUpDisplay.Current.LazyCreateSquadronIcons();
        HeadsUpDisplay.Current.RefreshSquadronIcons();
        HeadsUpDisplay.Current.ShowAlive();
        HeadsUpDisplay.Current.UpdateSpaceJunk();

        Squadron.Cycle(0);

        SetControlEnabled(false);
        HeadsUpDisplay.Current.HideCrosshair();
        /*
        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
        cam.SetState(cam.Pan);
        */
        _playerNpc.enabled = true;
        _playerNpc.PathDestination = SpawnPathDestination.UniversePosition;
        _playerNpc.SetState(_playerNpc.Path);
        StartCoroutine(DelayedControlEnable(SpawnControlDelay));
    }

    private IEnumerator DelayedControlEnable(float delay)
    {
        yield return new WaitForSeconds(delay);
        _playerNpc.SetState(_playerNpc.Idle);
        SetControlEnabled(true);
        _playerNpc.enabled = false;
        HeadsUpDisplay.Current.Show();
        //HeadsUpDisplay.Current.ShowCrosshair();

        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
        cam.SetState(cam.Follow);
    }

    private void SpawnVehicle(Vehicle vehiclePrefab, Shiftable spawner)
    {
        SpawnVehicle(vehiclePrefab, spawner.UniversePosition, spawner.transform.rotation);
    }

    private void SpawnVehicle(Vehicle vehiclePrefab, UniversePosition universePosition, Quaternion rotation)
    {
        _playerVehicleInstance = Instantiate(vehiclePrefab.gameObject, universePosition.CellLocalPosition, rotation).GetComponent<Vehicle>();
        _playerVehicleInstance.Controller = gameObject;
        _playerVehicleInstance.Shiftable.SetShiftPosition(universePosition);
        _playerVehicleInstance.GetComponent<Targetable>().Team = Team;

        var member = GetComponent<Fighter>();
        member.SetVehicleInstance(_playerVehicleInstance);

        _playerVehicleInstance.Initialize();

        var playerCurrent = Squadron.GetMember(Squadron.GetCurrentIndex()).GetComponent<ShipProfile>();

        if (playerCurrent.PrimaryWeapon != null)
            _playerVehicleInstance.SetPrimaryWeapon(playerCurrent.PrimaryWeapon);
        if (playerCurrent.SecondaryWeapon != null)
            _playerVehicleInstance.SetSecondaryWeapon(playerCurrent.SecondaryWeapon);

        _playerVehicleInstance.UTurnPath = UTurnPath;
        _playerVehicleInstance.UTurnDuration = UTurnDuration;
        //Destroy(_playVehicleInstance.GetComponent<Tracker>());

        Destroy(_playerVehicleInstance.GetComponent<VehicleTracker>());
        var squadronTracker = _playerVehicleInstance.gameObject.AddComponent<SquadronTracker>();
        squadronTracker.Options = Squadron.TrackerOptions;
        squadronTracker.CallSign = CallSign;
        squadronTracker.LabelFont = Squadron.SquadronTrackerFont;
        squadronTracker.IsDisabled = true;

        var mapPin = VehicleInstance.gameObject.AddComponent<MapPin>();
        mapPin.ActivePin = PlayerPinPrefab;
        mapPin.InactivePin = Squadron.SquadronPinPrefab;

        _playerVehicleInstance.gameObject.layer = LayerMask.NameToLayer("Player");

        _playerVehicleInstance.Killable.OnDamage += Player_OnDamage;
        _playerVehicleInstance.Killable.OnDie += Player_OnDie;

        _playerNpc.enabled = false;

        // Apply power profile
        var powerProfile = GetComponent<ShipProfile>();
        _playerVehicleInstance.Killable.MaxShield = powerProfile.GetShield();
        _playerVehicleInstance.Killable.Shield = _playerVehicleInstance.Killable.MaxShield;

        var shieldRegenerator = _playerVehicleInstance.gameObject.GetComponent<ShieldRegenerator>();
        shieldRegenerator.RegenerationDelay = Squadron.ShieldRegenerateDelay;
        shieldRegenerator.RegenerationRate = Squadron.ShieldRegenerateRate;
        shieldRegenerator.OnRegenerate += Player_OnRegenerate;
    }


    public void SetControlEnabled(bool value)
    {
        _controlEnabled = value;
        if (!value)
        {
            _playerVehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            _playerVehicleInstance.PrimaryWeaponInstance.IsTriggered = false;
            _playerVehicleInstance.SetAimAt(_playerVehicleInstance.GetAimPosition());
            _playerVehicleInstance.RollThrottle = 0f;
            _playerVehicleInstance.YawThrottle = 0f;
            _playerVehicleInstance.PitchThotttle = 0f;
            _playerVehicleInstance.TriggerBoost = false;
            _playerVehicleInstance.TriggerAccelerate = false;
            _playerVehicleInstance.TriggerBrake = false;
        }
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
    private UniversePosition _spawnPos;
    private Collider[] colliders = new Collider[10];

    private void Update()
    {
        if (_controlEnabled)
        {
            if (_playerVehicleInstance != null)
            {
                _vehicleController.ControlVehicle(VehicleInstance);
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

            _vehicleController.ControlSquadron(Squadron, _isAllowRespawn);

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
        }

        Vehicle leaderVehicle;
        var playerSquadronIndex = Squadron.GetCurrentIndex();
        if (playerSquadronIndex == 0)
        {
            leaderVehicle = VehicleInstance;
        }
        else
        {
            _leader = Squadron.GetMember(playerSquadronIndex);
            leaderVehicle = _leader.VehicleInstance;

            if (leaderVehicle != null && _playerVehicleInstance != null)
            {
                _leader.IdleDestination = leaderVehicle.transform.position + leaderVehicle.transform.forward * 10f;
                var leaderToPlayer = leaderVehicle.Shiftable.transform.position - _playerVehicleInstance.transform.position;
                if (leaderToPlayer.sqrMagnitude > 1000f * 1000f)
                    _leader.IdleDestination = _playerVehicleInstance.transform.position;
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

            // Swap player controlled vehicle as leader.
            if (playerSquadronIndex != 0)
            {
                var formationOffset = Formations.GetArrowOffset(playerSquadronIndex, 10f);
                Squadron.GetMember(0).IdleDestination = leaderVehicle.transform.position + leaderVehicle.transform.rotation * formationOffset;
                Squadron.GetMember(0).IdleUpDestination = leaderVehicle.transform.up;
            }
        }

        if (_playerVehicleInstance != null)
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

                    var detected = Physics.OverlapSphere(_playerVehicleInstance.transform.position, ThreatRadius, _detectableMask);
                    _threatCount = detected.Count(d => d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>() != null && d.GetComponent<Detectable>().TargetTransform.GetComponent<Targetable>().Team == Targeting.GetEnemyTeam(Team));
                    _threatCheckCooldown = 1f;

                    if (_threatCount == 0)
                        _noThreatCooldown = 0f;
                }
            }

            /*
			if (_threatCount > 0)
			{
				_noThreatCooldown = NoThreatTime;
			}
			else
			{
				if (_noThreatCooldown > 0f)
				{
					_noThreatCooldown -= Time.deltaTime;
				}
			}
            */

            if (_noThreatCooldown >= 0f)
                _noThreatCooldown -= Time.deltaTime;

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

        if (_playerVehicleInstance != null)
        {
            Debug.DrawLine(_playerVehicleInstance.transform.position, _playerVehicleInstance.transform.position + Vector3.up * 100f, Color.magenta);
        }
    }

    public void Respawn()
    {
        if (_playerVehicleInstance != null)
            _playerVehicleInstance.Killable.Die(Vector3.zero, Vector3.up, gameObject);
        Debug.Log("RESPAWN");
        var respawnAt = SpawnManager.FindNearest(_lastDeathUniversePosition);
        respawnAt.Spawn();
        _isAllowRespawn = false;
        HeadsUpDisplay.Current.RefreshSquadronIcons();
        HeadsUpDisplay.Current.ShowAlive();
        HeadsUpDisplay.Current.ShowCrosshair();
    }

    public void SetCallSign(string callsign)
    {
        _curCallSlign = callsign;
    }

    public string GetCallSign()
    {
        return _curCallSlign;
    }

    public void OnPlayerHit()
    {
        var viewPortPosition = Universe.Current.ViewPort.transform.position;
        ResourcePoolIndex.PlayAnonymousSound(PlayerHitSound, viewPortPosition, 0.3f, false);
        HeadsUpDisplay.Current.TriggerCrosshairPulse();
    }

    public void OnPlayerKill(Killable victim)
    {
        var killMessage = string.Empty;
        var vehicle = victim.GetComponent<Vehicle>();
        if (vehicle != null)
            killMessage = string.Format("{0} Destroyed", vehicle.Name);
        var turret = victim.GetComponent<Turret>();
        if (turret != null)
            killMessage = string.Format("{0} Destroyed", turret.Name);
        if (!string.IsNullOrEmpty(killMessage))
        {
            SpaceJunkCount += 10;
            HeadsUpDisplay.Current.ShowKillMessage(killMessage);
            Mission.Current.IncrementKills(1);
            Save();
            HeadsUpDisplay.Current.UpdateSpaceJunk();
        }
    }

    public void Give(string itemKey)
    {
        var playerFile = PlayerFile.ReadFromFile();
        if (playerFile.Inventory.Any(i => i.Key == itemKey))
        {
            playerFile.Inventory.First(i => i.Key == itemKey).BluePrintsOwned++;
        }
        else
        {
            playerFile.Inventory.Add(new PlayerFile.InventoryItem { Key = itemKey, BluePrintsOwned = 1, EquippedSlot = PlayerFile.EquippedSlot.Inventory, IsOwned = false });
        }
        playerFile.WriteToFile();
    }

    public void GiveShip(string shipKey)
    {
        var playerFile = PlayerFile.ReadFromFile();
        if (playerFile.Ships.Any(s => s.Key == shipKey))
        {
            playerFile.Ships.First(s => s.Key == shipKey).BluePrintsOwned++;
        }
        else
        {
            playerFile.Ships.Add(new PlayerFile.ShipItem { Key = shipKey, BluePrintsOwned = 1, IsOwned = false });
        }
        playerFile.WriteToFile();
    }

    public void Player_OnRegenerate()
    {
        HeadsUpDisplay.Current.RefreshSquadronIcon(0);
    }

    public void Player_OnDamage(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        AudioClip hitSound = null;
        var hitSoundVolumne = 1f;
        if (sender.Shield > 0f)
        {
            HeadsUpDisplay.Current.ShieldHit();
            if (ShieldGetHitSound != null && ShieldGetHitSound.Any())
            {
                hitSound = ShieldGetHitSound[Random.Range(0, ShieldGetHitSound.Count)];
                hitSoundVolumne = 0.2f;
            }
        }
        else
        {
            HeadsUpDisplay.Current.Hit();
            if (PlayerGetHitSound != null && PlayerGetHitSound.Any())
            {
                hitSound = PlayerGetHitSound[Random.Range(0, PlayerGetHitSound.Count)];
                hitSoundVolumne = 0.8f;
            }
        }

        if (hitSound != null)
        {
            var viewPortPosition = Universe.Current.ViewPort.transform.position;
            ResourcePoolIndex.PlayAnonymousSound(hitSound, viewPortPosition, hitSoundVolumne, false);
        }

        //Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.3f, 0.7f, 0.1f);
        HeadsUpDisplay.Current.RefreshSquadronIcon(0);
        if (VehicleInstance.Killable.Shield > 0f)
        {
            HeadsUpDisplay.Current.TriggerShieldHit();
        }
        else
        {
            HeadsUpDisplay.Current.TriggerHealthHit();
        }
        HeadsUpDisplay.Current.UpdateBars();
    }

    public void Player_OnDie(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        _noThreatCooldown = NoThreatTime;
        _deathCooldown = DeathOptionTime;
        _lastDeathUniversePosition = new UniversePosition(_playerVehicleInstance.Shiftable.UniversePosition.CellIndex, _playerVehicleInstance.Shiftable.UniversePosition.CellLocalPosition);
        Debug.Log("PLAYER VEHICLE DESTROYED AT: " + _lastDeathUniversePosition.CellIndex);
        HeadsUpDisplay.Current.RefreshSquadronIcon(0);
        HeadsUpDisplay.Current.TriggerCrosshairFadeOut();
        HeadsUpDisplay.Current.HideCrosshair();
        HeadsUpDisplay.Current.ShowDead();
        if (attacker != null)
        {
            var attackerTargetable = attacker.GetComponentInChildren<Targetable>();
            if (attackerTargetable != null)
                HeadsUpDisplay.Current.RecordKill(attackerTargetable.Team);

            var attackerName = string.Empty;
            var attackerVehicle = attacker.GetComponent<Vehicle>();
            if (attackerVehicle != null)
                attackerName = attackerVehicle.Name;
            var attackerTurret = attacker.GetComponent<Turret>();
            if (attackerTurret != null)
                attackerName = attackerTurret.Name;
            if (!string.IsNullOrEmpty(attackerName))
            {
                HeadsUpDisplay.Current.ShowKilledMessage(string.Format("YOU WERE KILLED BY {0}", attackerName).ToUpperInvariant());
            }
            else
            {
                HeadsUpDisplay.Current.ShowKilledMessage("YOU DIED");
            }
        }
        Universe.Current.ViewPort.GetComponent<VehicleCamera>().TriggerShake(0.3f, 0.7f, 0.5f);
    }

    public void ResetThreatCooldown()
    {
        _noThreatCooldown = NoThreatTime;
    }

    public bool InPlayerActiveCells(CellIndex checkCell)
    {
        var playerCellIndex = _playerVehicleInstance.Shiftable.UniverseCellIndex;
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

    public Vehicle VehicleInstance
    {
        get { return _playerVehicleInstance; }
    }

    public void SetVehicleInstance(Vehicle vehicle)
    {
        _playerVehicleInstance = vehicle;
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

    private BluePrint BluePrintFromWeapon(WeaponDefinition weapon)
    {
        Debug.Log("KEY: " + weapon.Key);
        return BluePrintPool.All().First(b => b.ItemAs<WeaponDefinition>().Key == weapon.Key);
    }

    public PlayerFile BuildFile()
    {
        if (_profile.PrimaryWeapon == null)
            _profile.PrimaryWeapon = VehicleInstance.PrimaryWeapon;
        if (_profile.SecondaryWeapon == null)
            _profile.SecondaryWeapon = VehicleInstance.SecondaryWeapon;

        var ships = new List<PlayerFile.ShipItem>();
        ships.Add(new PlayerFile.ShipItem { Key = VehicleInstance.Key, IsOwned = true });

        var inventory = new List<PlayerFile.InventoryItem>();
        var primaryBluePrint = BluePrintFromWeapon(_profile.PrimaryWeapon);
        inventory.Add(new PlayerFile.InventoryItem { Key = primaryBluePrint.Key, BluePrintsOwned = primaryBluePrint.RequiredCount, EquippedSlot = PlayerFile.EquippedSlot.Primary, IsOwned = true });

        var secondaryBluePrint = BluePrintFromWeapon(_profile.SecondaryWeapon);
        inventory.Add(new PlayerFile.InventoryItem { Key = secondaryBluePrint.Key, BluePrintsOwned = secondaryBluePrint.RequiredCount, EquippedSlot = PlayerFile.EquippedSlot.Secondary, IsOwned = true });

        return new PlayerFile
        {
            Ship = VehicleInstance.Key,
            SpaceJunk = SpaceJunkCount,
            Ships = ships,
            Inventory = inventory
        };
    }

    public void Save()
    {
        var playerFile = PlayerFile.ReadFromFile();
        playerFile.SpaceJunk = SpaceJunkCount;
        playerFile.WriteToFile();
    }

    public void Load()
    {
        var playerFile = PlayerFile.ReadFromFile();
        SpaceJunkCount = playerFile.SpaceJunk;
        var primaryInventoryItem = playerFile.Inventory.First(i => i.EquippedSlot == PlayerFile.EquippedSlot.Primary);
        var secondaryInventoryItem = playerFile.Inventory.First(i => i.EquippedSlot == PlayerFile.EquippedSlot.Secondary);

        Debug.Log("EQUIPPED:");
        Debug.Log("PRIMARY: " + primaryInventoryItem.Key);
        Debug.Log("SECONDARY: " + secondaryInventoryItem.Key);

        _profile.PrimaryWeapon = BluePrintPool.ByKey(primaryInventoryItem.Key).ItemAs<WeaponDefinition>();
        _profile.SecondaryWeapon = BluePrintPool.ByKey(secondaryInventoryItem.Key).ItemAs<WeaponDefinition>();

        _playerVehiclePrefab = VehiclePool.ByKey(playerFile.Ship);
        SpawnVehicle(_playerVehiclePrefab, Universe.Current.PlayerSpawnPosition);
        VehicleInstance.SetPrimaryWeapon(_profile.PrimaryWeapon);
        VehicleInstance.SetSecondaryWeapon(_profile.SecondaryWeapon);
    }
}
