using UnityEngine;

public class ResettableEffect : MonoBehaviour {
    private Shiftable _shiftable;

    public AudioSource Sound;

    void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        foreach (var ps in _shiftable.ShiftParticleSystems)
        {
            ps.Stop();
        }
    }

    void Start()
    {
        var poolItem = GetComponent<ResourcePoolItem>();
        if (poolItem != null)
        {
            poolItem.OnGetAvaiable = () =>
            {
                Reset();
            };
        }
    }

    public void Reset()
    {
        foreach (var ps in _shiftable.ShiftParticleSystems)
        {
            ps.Play();
        }
        Sound.Play();
    }
}
