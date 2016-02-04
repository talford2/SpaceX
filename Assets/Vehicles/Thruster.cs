using UnityEngine;

public class Thruster : MonoBehaviour
{
    public FlareDistanceBrightness Flare;
    public ShiftTrail Trail;

    public void Initialize()
    {
        Trail.Initialize(transform.position);
    }
}
