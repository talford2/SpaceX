using System;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{

	public float Radius;

	private int _detectableMask;

	public void Awake()
	{
		_detectableMask = LayerMask.GetMask("Detectable");
	}

	public void Detect(Action<Transform> action)
	{
		var detectables = Physics.OverlapSphere(transform.position, Radius, _detectableMask);
		foreach (var detected in detectables)
		{
			var detectable = detected.GetComponent<Detectable>();
			if (detectable != null)
				action(detectable.TargetTransform);
		}
	}
}
