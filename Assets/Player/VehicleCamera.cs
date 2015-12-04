using System.Collections.Generic;
using UnityEngine;

public class VehicleCamera : UniverseCamera
{
	public Vehicle Target;

	public Transform BackgroundTransform;

	public List<Camera> ChildCameras;

	[Header("Offset")]
	public float DistanceBehind = 8f;
	public float VerticalDistance = 2f;

	[Header("Spring")]
	public float SpringCompression = 0.5f;
	public float SpringExpansion = 1.5f;
	public float SpringBoostExpansion = 3f;

	[Header("Lerp Speeds")]
	public float RotationCatchup = 3f;
	public float SpringCatchup = 2f;
	public float OffsetCatchup = 5f;

	//private Camera attachedCamera;
	private float springDistance;
	private Quaternion offsetAngle;
	private Vector3 offset;

	private float targetFov;
	private Vector3 targetUp;
	
	private void Start()
	{
		springDistance = 1f;
		Target = PlayerController.Current.VehicleInstance;
		offsetAngle = Target.transform.rotation;
		targetUp = Target.transform.up;
	}

	public override void Move()
	{
		if (Target != null)
		{
			var targetSpringDistance = 1f;
			targetFov = 60f;
			if (Target.IsBoosting)
			{
				targetSpringDistance = SpringBoostExpansion;
				targetFov = 100f;
			}
			else
			{
				if (Target.IsAccelerating)
				{
					targetSpringDistance = SpringExpansion;
				}
				else
				{
					if (Target.IsBraking)
					{
						targetSpringDistance = SpringCompression;
					}
				}
			}

			springDistance = Mathf.Lerp(springDistance, targetSpringDistance, SpringCatchup * Time.deltaTime);

			offsetAngle = Quaternion.Lerp(offsetAngle, Target.transform.rotation, RotationCatchup * Time.deltaTime);
			offset = Vector3.Lerp(offset, offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * springDistance, OffsetCatchup * Time.deltaTime);

			_shiftable.Translate(Target.transform.position + offset - transform.position);

			targetUp = Vector3.Lerp(targetUp, Target.transform.up, 5f * Time.deltaTime);

			transform.LookAt(Target.GetAimPosition(), targetUp);
		}

		BackgroundTransform.transform.position = transform.position;

		AttachedCamera.fieldOfView = Mathf.Lerp(AttachedCamera.fieldOfView, targetFov, Time.deltaTime);

		foreach (var childCam in ChildCameras)
		{
			childCam.fieldOfView = AttachedCamera.fieldOfView;
		}

		// Shake
		var offsetDestination = Vector3.zero;
		var angleDestination = Quaternion.identity;
		if (isShaking)
		{
			if (shakeCooldown >= 0f)
			{
				shakeCooldown -= Time.deltaTime;
				shakeFrequencyCooldown -= Time.deltaTime;
				var shakeFraction = Mathf.Clamp01(shakeCooldown / shakeDuration);
				if (shakeFrequencyCooldown < 0f)
				{
					offsetDestination = Random.onUnitSphere * shakeAmplitude * shakeFraction;
					angleDestination = Quaternion.AngleAxis(Random.Range(-40f, 40f), AttachedCamera.transform.forward);
					shakeFrequencyCooldown = shakeFrequency;
				}
				if (shakeCooldown < 0f)
					isShaking = false;
			}
		}

		AttachedCamera.transform.localPosition = Vector3.Lerp(AttachedCamera.transform.localPosition, offsetDestination, 2f * Time.deltaTime);
		AttachedCamera.transform.localRotation = Quaternion.Lerp(AttachedCamera.transform.localRotation, angleDestination, 2f * Time.deltaTime);
	}

	private bool isShaking;
	private float shakeAmplitude;
	private float shakeDuration;
	private float shakeCooldown;
	private float shakeFrequency;
	private float shakeFrequencyCooldown;

	public void TriggerShake(float amplitude, float duration, float frequency)
	{
		//isShaking = true;
		shakeAmplitude = amplitude;
		shakeDuration = duration;
		shakeCooldown = shakeDuration;
		shakeFrequency = frequency;
		shakeFrequencyCooldown = shakeFrequency;
	}

	public void Reset()
	{
		offsetAngle = Target.transform.rotation;
		springDistance = 1f;
		offset = offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * springDistance;
		targetUp = Target.transform.up;

		transform.position = Target.transform.position + offset;
		transform.LookAt(Target.transform, Target.transform.up);
		//Debug.Break();
	}
}
