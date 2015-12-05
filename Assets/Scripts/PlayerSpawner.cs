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
        var universePosition = new UniversePosition(Shiftable.UniversePosition.CellIndex, Shiftable.UniversePosition.CellLocalPosition + localOffset);
        Universe.Current.WarpTo(universePosition);
        PlayerController.Current.SpawnVehicle(PlayerController.Current.VehiclePrefab, universePosition, transform.rotation);
        PlayerController.Current.VehicleInstance.transform.position = Universe.Current.GetWorldPosition(universePosition);
        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
        cam.Target = PlayerController.Current.VehicleInstance;
        cam.Reset();
    }

    private void OnDestroy()
    {
        SpawnManager.RemoveSpawner(this);
    }
}
