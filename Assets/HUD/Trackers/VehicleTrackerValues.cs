using UnityEngine;

[CreateAssetMenu(fileName = "VehicleTracker", menuName = "SpaceX/Tracker", order = 1)]
public class VehicleTrackerValues : ScriptableObject
{
    public Sprite ArrowSprite;
    public Color TrackerColor = Color.white;
    public Sprite LockingSprite;
    public Sprite LockedSprite;

    public Color BackgroundColor = new Color(0f, 0f, 0f, 0.5f);
    public Color ShieldBarColor = new Color(0.6f, 0.6f, 1f, 1f);
    public Color HealthBarColor = new Color(1f, 1f, 1f, 1f);

    public GameObject TrackerPlanePrefab;
    public float TrackerPlaneScale = 100f;

    public float MaxDistance = 3000f;
}
