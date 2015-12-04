using UnityEngine;
using System.Collections;

public class WarpEffect : MonoBehaviour
{
	private float _naturalScale = 1f;

	public float Timeout = 1f;

	public float Distance = 20f;

	public GameObject WarpEffectPrefab;
	
	private float _cooldown = 0;

	private Vector3 _finalDestination;

	private Vector3 _warpPos;

	void Awake()
	{
		_naturalScale = transform.localScale.x;
		_finalDestination = transform.position;

		_warpPos = transform.position - (transform.forward * Distance);

		_cooldown = Timeout;
	}

	void Update()
	{
		// 0 to 1
		var frac = 1 - _cooldown / Timeout;

		transform.position = Vector3.Lerp(_warpPos, _finalDestination, frac);
		transform.localScale = _naturalScale * Vector3.one * frac;
		_cooldown -= Time.deltaTime;
		if (_cooldown < 0)
		{
			Destroy(this);
		}
	}
}
