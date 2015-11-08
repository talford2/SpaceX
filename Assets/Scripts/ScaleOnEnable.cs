using UnityEngine;
using System.Collections;

public class ScaleOnEnable : MonoBehaviour
{
	public float AppearTime = 1f;
	float _coolDown = 0f;

	void Update()
	{
		if (_coolDown < AppearTime)
		{
			_coolDown += Time.deltaTime;
			_coolDown = Mathf.Min(_coolDown, AppearTime);

			transform.localScale = Vector3.one * (_coolDown / AppearTime);
		}
	}

	void OnEnable()
	{
		transform.localScale = Vector3.zero;
		_coolDown = 0f;
	}
}
