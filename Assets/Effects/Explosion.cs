using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	private Shiftable _shiftable;

	public AudioSource ExplosionSound;

	void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		foreach (var ps in _shiftable.ShiftParticleSystems)
		{
			ps.Stop();
		}
	}

	public void Reset()
	{
		foreach (var ps in _shiftable.ShiftParticleSystems)
		{
			ps.Play();
			ExplosionSound.Play();
		}
	}
}
