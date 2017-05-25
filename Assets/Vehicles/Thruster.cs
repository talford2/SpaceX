using UnityEngine;

public class Thruster : MonoBehaviour
{
    public FlareDistanceBrightness Flare;
    public ShiftTrail Trail;
    public ParticleSystem BoomParticles;
    public ParticleSystem ContinousThrust;
    public AudioSource BoostStartSound;
    public AudioSource BoostLoopSound;

    public void Initialize()
    {
        Trail.Shiftable = GetComponentInParent<Shiftable>();
        Trail.Initialize(transform.position);
    }

    public void TriggerBoost()
    {
        if (BoomParticles != null && !BoomParticles.isPlaying)
        {
            BoomParticles.Play();
        }
        if (BoostStartSound != null)
        {
            BoostStartSound.Play();
        }
        if (ContinousThrust != null && !ContinousThrust.isPlaying)
        {
            ContinousThrust.Play();
        }
        if (BoostLoopSound != null)
        {
            BoostLoopSound.Play();
        }
    }

    public void StopBoost()
    {
        if (ContinousThrust != null)
        {
            ContinousThrust.Stop();
        }
        if (BoostLoopSound != null)
        {
            BoostLoopSound.Stop();
        }
    }
}
