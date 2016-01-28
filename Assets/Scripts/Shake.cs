using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
	#region Public Variables

	public float PositionAmplitude = 0.3f;

	public float RotateAmplitude = 1f;

	public float frequency = 10f;

	public float Duration = 2f;

	public AnimationCurve ShakeAmplitude;

	#endregion

	#region Private Variables

	private float _cooldown = 0;

	#endregion

	void Update()
	{
		if (_cooldown > 0)
		{
			var frac = 1f - _cooldown / Duration;

			var amp = ShakeAmplitude.Evaluate(frac);

			var freq = frequency * Mathf.Rad2Deg / 90f;

			var x = Mathf.Sin(freq);
			var y = Mathf.Sin(freq * 1.2f);
			var z = Mathf.Sin(freq * 1.4f);

			var fracVec = new Vector3(x, y, z) * amp;

			transform.localPosition = fracVec * PositionAmplitude;
			transform.localRotation = Quaternion.Euler(fracVec * RotateAmplitude);

			_cooldown -= Time.deltaTime;
			_cooldown = Mathf.Max(_cooldown, 0);
		}
	}

	public void TriggerShake(float duration, float amplitude)
	{
		PositionAmplitude = amplitude;
		_cooldown = duration;
	}
}
