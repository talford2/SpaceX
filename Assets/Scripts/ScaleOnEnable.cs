using UnityEngine;

public class ScaleOnEnable : MonoBehaviour
{
	public float AppearTime = 1f;
	private float _coolDown = 0f;
	private float _size;

	void Update()
	{
		if (_coolDown < AppearTime)
		{
			_coolDown += Time.deltaTime;
			_coolDown = Mathf.Min(_coolDown, AppearTime);

			transform.localScale = Vector3.one * _size * (_coolDown / AppearTime);
		}
	}

	public void ResetToZero(float size)
	{
		_size = size;
		transform.localScale = Vector3.zero;
		_coolDown = 0f;
	}
}
