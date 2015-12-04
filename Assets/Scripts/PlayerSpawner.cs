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
        Universe.Current.WarpTo(Shiftable.UniversePosition);
        PlayerController.Current.SpawnVehicle(PlayerController.Current.VehiclePrefab, Shiftable);
        var cam = Universe.Current.ViewPort.GetComponent<VehicleCamera>();
        cam.Target = PlayerController.Current.VehicleInstance;
        cam.Reset();
    }

    private void OnDestroy()
    {
        SpawnManager.RemoveSpawner(this);
    }
}
