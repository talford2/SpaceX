using UnityEngine;

public class Fighter : Npc<Fighter>
{
	public Vehicle VehiclePrefab;
    public Team Team;
    public bool IsSquadronMember;

	private Vehicle _vehicleInstance;
	public Vector3 Destination;

	public Transform Target;
    public float MaxTargetDistance = 2000f;

	public float SteerMultiplier = 0.3f;

    public ProximitySensor ProximitySensor;

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

        Steering = new FighterSteering(this);
        State = new FighterIdle(this);
    }

    private void Update()
    {
        if (VehicleInstance != null)
            UpdateState();
    }

    public void SpawnVehicle(Vehicle vehiclePrefab, UniversePosition universePosition)
    {
        _vehicleInstance = Instantiate<Vehicle>(vehiclePrefab);
        _vehicleInstance.GetComponent<Targetable>().Team = Team;
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

    private void OnVehicleDestroyed(Killable sender)
    {
        Target = null;
        if (!IsSquadronMember)
            Destroy(gameObject);
    }

    private void OnGUI()
	{
		GUI.Label(new Rect(Screen.width - 150f, 50f, 100f, 30f), State.Name);
		GUI.Label(new Rect(Screen.width - 150f, 80f, 100f, 30f), string.Format("{0:f2}", VehicleInstance.GetVelocity().magnitude));
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
			//Gizmos.DrawLine(VehicleInstance.transform.position, Destination);
			//Gizmos.DrawSphere(Destination, 2f);
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
