using System.Collections.Generic;
using UnityEngine;

public class ResettableEffect : MonoBehaviour
{
	private Shiftable _shiftable;

	public AudioSource Sound;

	public List<ParticleSystem> ShiftParticleSystems;
	
	void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
        foreach (var ps in ShiftParticleSystems)
        {
            if (ps != null)
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
        foreach (var ps in ShiftParticleSystems)
        {
            if (ps != null)
                ps.Play();
        }
        Sound.Play();
    }
}
