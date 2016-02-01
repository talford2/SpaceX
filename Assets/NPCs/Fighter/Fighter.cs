using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fighter : Npc<Fighter>
{
	public Vehicle VehiclePrefab;
    public Team Team;
    public bool IsSquadronMember;
    public string CallSign;
    [Header("Drop Item on Death")]
    public Collectible DropItem;
    public int MaxDropAmount = 5;

    private Vehicle _vehicleInstance;

    [Header("Movement")]
	public Vector3 Destination;

	public Transform Target;
    public float MaxTargetDistance = 2000f;

	public float SteerMultiplier = 0.3f;

    public ProximitySensor ProximitySensor;
    public bool IsDebugSpawn;

    [Header("Idle Destination")]
    public bool IsFollowIdleDestination;
    public Vector3 IdleDestination;
    public Vector3 IdleUpDestination;

	[Header("Attack")]
	public float AttackRange = 100f;
	public float ShootAngleTolerance = 5f;
	public float OvertakeDistance = 50f;
    public float BurstTime = 0.5f;
    public float BurstWaitTime = 0.7f;
    public float ExrapolationTimeError = 0.5f;
    public float AimOffsetRadius = 5f;

	[Header("Evade")]
	public float EvadeDistance = 200f;
	public float TurnAroundDistance = 250f;
	public float AcclerateDistance = 100f;
	public float MinDodgeIntervalTime = 1f;
	public float MaxDodgeIntervalTime = 6f;
	public float DodgeRadius = 50f;
	public float DodgeArcAngle = 180f;

	[Header("Chase")]
	public float SightRange = 50f;

	public Vehicle VehicleInstance { get { return _vehicleInstance; } }
    public FighterSteering Steering { get; set; }
    public Action<Transform> OnVehicleDamage;

    private void Awake()
    {
        //_vehicleInstance = Utility.InstantiateInParent(VehiclePrefab.gameObject, transform).GetComponent<Vehicle>();

        if (VehiclePrefab != null)
        {
            var parentShifter = transform.GetComponentInParent<Shiftable>();
            if (parentShifter != null)
            {
                SpawnVehicle(VehiclePrefab, parentShifter.UniversePosition);
            }
            else
            {
                // This isn't right!
                //SpawnVehicle(VehiclePrefab, new UniversePosition(new CellIndex(0,0,0), new Vector3()));
            }
        }
		
		CallSign = NameGenerator.GetRandomCallSign();

        Steering = new FighterSteering(this);
        State = new FighterIdle(this);

        if (IsDebugSpawn)
        {
            SpawnVehicle(VehiclePrefab, new UniversePosition(new CellIndex(0, 0, 0), new Vector3(0, 0, 0)));
            _vehicleInstance.GetComponent<VehicleTracker>().enabled = false;
        }
    }

    private void Update()
    {
        if (VehicleInstance != null)
            UpdateState();

        if (VehicleInstance != null)
        {
            var pitchYaw = GetPitchYawToPoint(Destination);
            if (pitchYaw.sqrMagnitude <= 0f)
            {
                // Give random value to resolve zero pitchYaw issue.
                pitchYaw = Random.insideUnitCircle;
            }
            VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
            VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;
        }
    }

    public void SpawnVehicle(Vehicle vehiclePrefab, UniversePosition universePosition)
    {
        _vehicleInstance = Instantiate<Vehicle>(vehiclePrefab);
        _vehicleInstance.GetComponent<Targetable>().Team = Team;
        _vehicleInstance.GetComponent<Killable>().OnDamage += OnVehicleDamaged;
        _vehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;
        _vehicleInstance.Shiftable.SetShiftPosition(universePosition);
        _vehicleInstance.transform.position = _vehicleInstance.Shiftable.GetWorldPosition();
        ProximitySensor = _vehicleInstance.GetComponent<ProximitySensor>();
    }

    public void SetVehicleInstance(Vehicle vehicleInstance)
    {
        _vehicleInstance = vehicleInstance;
        //_vehicleInstance.GetComponent<Targetable>().Team = Team;
        _vehicleInstance.GetComponent<Killable>().OnDie += OnVehicleDestroyed;
        ProximitySensor = _vehicleInstance.GetComponent<ProximitySensor>();
    }

	public Vector2 GetPitchYawToPoint(Vector3 point)
	{
		var toPoint = point - VehicleInstance.transform.position;
		var yawAmount = Vector3.Dot(toPoint.normalized, VehicleInstance.transform.right);
		var pitchAmount = Vector3.Dot(-toPoint.normalized, VehicleInstance.transform.up);
		return new Vector2(pitchAmount, yawAmount);
	}

    private void OnVehicleDamaged(Vector3 position, Vector3 normal, GameObject attacker)
    {
        if (attacker != null)
        {
            if (attacker.transform != null)
            {
                if (Target == null)
                    Target = attacker.transform;
                if (OnVehicleDamage != null)
                    OnVehicleDamage(attacker.transform);
            }
        }
    }

    private void OnVehicleDestroyed(Killable sender)
    {
        Target = null;
        if (DropItem != null)
        {
            var dropAmount = Random.Range(0, MaxDropAmount);
            for (var i = 0f; i < dropAmount; i++)
            {
                var dropPosition = VehicleInstance.transform.position + Random.onUnitSphere*1.5f;
                var dropItem = ((GameObject) Instantiate(DropItem.gameObject, VehicleInstance.transform.position + Random.onUnitSphere*1.5f, Quaternion.identity)).GetComponent<Collectible>();
                dropItem.Shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(dropPosition));
                dropItem.SetVelocity(VehicleInstance.GetVelocity() + Random.onUnitSphere*5f);
            }
        }
        if (!IsSquadronMember)
            Destroy(gameObject);
    }

	private void OnDrawGizmos()
	{
		if (VehicleInstance != null)
		{
			switch (State.Name)
			{
                case "Idle":
			        Gizmos.color = Color.white;
			        break;
				case "Evade":
					Gizmos.color = Color.magenta;
					break;
				case "Chase":
					Gizmos.color = Color.red;
					break;
				case "Attack":
					Gizmos.color = Color.yellow;
					break;
			}
			Gizmos.DrawLine(VehicleInstance.transform.position, Destination);
			Gizmos.DrawSphere(Destination, 2f);
            /*
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(VehicleInstance.transform.position, SightRange);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(VehicleInstance.transform.position, AttackRange);
            */
			Gizmos.color = Color.green;
			Gizmos.DrawLine(VehicleInstance.transform.position, VehicleInstance.transform.position + VehicleInstance.transform.forward * 100f);
        }
	}
}
