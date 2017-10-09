using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Shiftable Shiftable;

    private void Start()
    {
        SpawnManager.AddSpawner(this);
    }

    public void Spawn()
    {
        var localOffset = transform.rotation*transform.localPosition;
        var universePosition = new UniversePosition(Shiftable.UniversePosition.CellIndex, Shiftable.UniversePosition.CellLocalPosition);// + localOffset);
        Universe.Current.WarpTo(universePosition);
        Player.Current.Squadron.SpawnSquadronVehicle(Player.Current.Squadron.GetCurrentMember(), universePosition, transform.rotation);
        Player.Current.Squadron.Cycle(0);
        Player.Current.VehicleInstance.transform.position = Universe.Current.GetWorldPosition(universePosition);
        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
        cam.Target = Player.Current.VehicleInstance;
        cam.Reset();
    }

    private void OnDestroy()
    {
        SpawnManager.RemoveSpawner(this);
    }
}
