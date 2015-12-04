using UnityEngine;
using System.Collections;

public class ScaleToSize : MonoBehaviour
{
	private float _naturalScale = 1f;

	public float Timeout = 1f;

	private float _cooldown = 0;

	void Awake()
	{
		_naturalScale = transform.localScale.x;
		_cooldown = Timeout;
	}

	void Update()
	{
		var frac = 1 - _cooldown / Timeout;
		transform.localScale = _naturalScale * Vector3.one * frac;
		_cooldown -= Time.deltaTime;
		if (_cooldown < 0)
		{
			Destroy(this);
		}
	}
}
