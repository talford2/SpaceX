public class Fighter1 : Npc<Fighter1>
{
    public Vehicle VehiclePrefab;

    private Vehicle _vehicleInstance;

    public Vehicle VehicleInstance { get { return _vehicleInstance; } }

    private void Awake()
    {
        _vehicleInstance = Utility.InstantiateInParent(VehiclePrefab.gameObject, transform).GetComponent<Vehicle>();
        State = new Fighter1Chase(this);
    }

    private void Update()
    {
        UpdateState();
    }
}
