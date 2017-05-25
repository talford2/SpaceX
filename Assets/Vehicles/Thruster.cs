using UnityEngine;

public class Thruster : MonoBehaviour
{
    public FlareDistanceBrightness Flare;
    public ShiftTrail Trail;
    public ParticleSystem BoomParticles;

    public void Initialize()
    {
        Trail.Initialize(transform.position);
    }

    public void TriggerBoost()
    {
        if (BoomParticles != null && !BoomParticles.isPlaying)
        {
            BoomParticles.Play();
        }
    }
}
