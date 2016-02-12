public class CollectibleEvent : UniverseEvent
{
    public Collectible CollectiblePrefab;

    private MapPin _mapPin;

    public override void Awake()
    {
        base.Awake();
        _mapPin = GetComponent<MapPin>();
    }

    public override void Trigger()
    {
        var collectible = Instantiate(CollectiblePrefab);
        collectible.Shiftable.SetShiftPosition(Shiftable.UniversePosition);
        collectible.transform.position = Universe.Current.GetWorldPosition(Shiftable.UniversePosition);
        collectible.OnCollect += Deactivate;
        base.Trigger();
    }

    public void Deactivate()
    {
        if (_mapPin != null)
            _mapPin.SetPinState(MapPin.MapPinState.Inactive);
    }
}
