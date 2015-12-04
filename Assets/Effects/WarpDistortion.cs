using UnityEngine;
using System.Collections;

public class WarpDistortion : MonoBehaviour
{
	private Material _warpMat;

	public float WarpTime = 1;

	public float WarpBumpAmount = 100f;

	private float _cooldown = 0;

	void Awake()
	{
		_warpMat = GetComponent<Renderer>().material;
	}

	void Start()
	{
		_cooldown = WarpTime;
	}

	void Update()
	{
		_cooldown -= Time.deltaTime;

		var frac = Mathf.Clamp(1f - _cooldown / WarpTime, 0, 1);

		_warpMat.SetFloat("_BumpAmt", Mathf.Sin(Mathf.Deg2Rad * 180 * frac) * WarpBumpAmount);

		if (_cooldown <= 0)
		{
			Destroy(gameObject);
		}
	}
}
