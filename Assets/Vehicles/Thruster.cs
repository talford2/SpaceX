using UnityEngine;

public class Thruster : MonoBehaviour
{
    public FlareDistanceBrightness Flare;
    public ShiftTrail Trail;
    public ParticleSystem BoomParticles;
    public ParticleSystem ContinousThrust;

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
        if (ContinousThrust != null && !ContinousThrust.isPlaying)
        {
            ContinousThrust.Play();
        }
    }

    public void StopBoost()
    {
        if (ContinousThrust != null)
        {
            ContinousThrust.Stop();
        }
    }
}
