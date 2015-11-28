using UnityEngine;

public class IdleRotation : MonoBehaviour
{
	public float RotationSpeed;

	private Vector3 _rotationVelocity;

	public Transform RotateTransform;

	private void Awake()
	{
		if (RotateTransform == null)
		{
			RotateTransform = transform;
		}

		RotateTransform.rotation *= Quaternion.Euler(RandomEuler(360f));
		_rotationVelocity = RandomEuler(RotationSpeed);
	}

	private void Update()
	{
		RotateTransform.rotation *= Quaternion.Euler(_rotationVelocity * Time.deltaTime);
	}

	private Vector3 RandomEuler(float magnitude)
	{
		var halfMagnitude = magnitude / 2f;
		return new Vector3(Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude), Random.Range(-halfMagnitude, halfMagnitude));
	}
}
