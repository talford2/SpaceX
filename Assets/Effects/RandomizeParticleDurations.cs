using UnityEngine;

public class RandomizeParticleDurations : MonoBehaviour {

    public ParticleSystem Particles;
    public float MinDuration;
    public float MaxDuration;

    private void Awake()
    {
        var psMain = Particles.main;
        psMain.duration = Random.Range(MinDuration, MaxDuration);
    }

    private void Start()
    {
        Particles.Play();
    }
}
