using UnityEngine;

public class ProximitySpawner : MonoBehaviour
{
    public Fighter FighterPrefab;
    public float Radius = 2000f;
    public float Interval = 20f;
    public Transform PathPoint;

    private float _radiusSquared;
    private float _intervalCooldown;

    private void Awake()
    {
        _radiusSquared = Radius*Radius;
        _intervalCooldown = Random.Range(0f, Interval);
    }

    private void Update()
    {
        if (PlayerController.Current != null && PlayerController.Current.VehicleInstance != null)
        {
            if (_intervalCooldown >= 0f)
            {
                _intervalCooldown -= Time.deltaTime;
                if (_intervalCooldown < 0f)
                {
                    var toPlayer = PlayerController.Current.VehicleInstance.transform.position - transform.position;
                    if (toPlayer.sqrMagnitude < _radiusSquared)
                    {
                        Spawn();
                    }
                    _intervalCooldown = Interval;
                }
            }
        }
    }

    public void Spawn()
    {
        var fighterInst = Instantiate(FighterPrefab);
        fighterInst.SpawnVehicle(fighterInst.VehiclePrefab, Universe.Current.GetUniversePosition(transform.position), transform.rotation);
        if (PathPoint != null)
        {
            fighterInst.SetPath(Universe.Current.GetUniversePosition(PathPoint.position));
        }
    }
}