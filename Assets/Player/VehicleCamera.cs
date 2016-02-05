﻿using System.Collections.Generic;
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

	public AnimationCurve ShakeAmplitude = AnimationCurve.Linear(0, 0, 1, 1);

	private float springDistance;
	private Quaternion offsetAngle;
	private Vector3 offset;

	private float targetFov;
	private Vector3 targetUp;

    // Shake Params
    private bool isShaking;
    private bool isContinuous;
    private bool isShakingTriggered;
    private float shakeAmplitude;
    private float shakeDuration;
    private float shakeCooldown;
    private float shakeFrequency;

    private float amplitudeFrac;
    private float shakeTime;

    private void Start()
	{
		springDistance = 1f;
		Target = PlayerController.Current.VehicleInstance;
		offsetAngle = Target.transform.rotation;
		targetUp = Target.transform.up;
        isShakingTriggered = false;
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

	        springDistance = Mathf.Lerp(springDistance, targetSpringDistance, SpringCatchup*Time.deltaTime);

	        offsetAngle = Quaternion.Lerp(offsetAngle, Target.transform.rotation, RotationCatchup*Time.deltaTime);
	        offset = Vector3.Lerp(offset, offsetAngle*new Vector3(0f, VerticalDistance, -DistanceBehind)*springDistance, OffsetCatchup*Time.deltaTime);

	        if (Target.PrimaryWeaponInstance != null)
	        {
	            _shiftable.Translate(Target.PrimaryWeaponInstance.GetShootPointCentre() + offset - transform.position);
	        }
	        else
	        {
	            _shiftable.Translate(Target.transform.position + offset - transform.position);
            }

            targetUp = Vector3.Lerp(targetUp, Target.transform.up, 5f*Time.deltaTime);

            transform.LookAt(Target.GetAimPosition(), targetUp);
	    }

	    BackgroundTransform.transform.position = transform.position;

		AttachedCamera.fieldOfView = Mathf.Lerp(AttachedCamera.fieldOfView, targetFov, Time.deltaTime);

		foreach (var childCam in ChildCameras)
		{
			childCam.fieldOfView = AttachedCamera.fieldOfView;
		}

		if (isShaking)
		{
		    if (!isContinuous)
		    {
		        if (shakeCooldown >= 0f)
		        {
		            shakeCooldown -= Time.deltaTime;
                    if (shakeCooldown < 0f)
                    {
                        AttachedCamera.transform.localPosition = Vector3.zero;
                        isShaking = true;
                    }

                    var frac = 1f - shakeCooldown/shakeDuration;
                    amplitudeFrac = ShakeAmplitude.Evaluate(frac);
                    var freq = shakeCooldown*Mathf.Rad2Deg* shakeFrequency;

                    Shake(amplitudeFrac, freq);
                }
            }
            else
		    {
		        if (isShakingTriggered)
		        {
		            amplitudeFrac = Mathf.Lerp(amplitudeFrac, 1f, 5f*Time.deltaTime);
		        }
		        else
		        {
                    amplitudeFrac = Mathf.Lerp(amplitudeFrac, 0f, 5f * Time.deltaTime);
                }
		        if (amplitudeFrac < 0.0001f)
		        {
                    AttachedCamera.transform.localPosition = Vector3.zero;
                    isShaking = false;
		        }

		        shakeTime += Time.deltaTime;
		        shakeTime -= Mathf.Floor(shakeTime);
                var freq = shakeTime * Mathf.Rad2Deg * shakeFrequency;

                Shake(amplitudeFrac, freq);

		        isShakingTriggered = false;
            }
		}
	}

    private void Shake(float amplitudeFraction, float frequency)
    {
        var x = Mathf.Sin(frequency);
        var y = Mathf.Sin(frequency*1.2f);
        var z = Mathf.Sin(frequency*1.4f);

        var fracVec = new Vector3(x, y, z)*amplitudeFraction;

        AttachedCamera.transform.localPosition = shakeAmplitude*fracVec;
        AttachedCamera.transform.localRotation = Quaternion.Euler(2f*Mathf.PI*shakeAmplitude*fracVec);
    }

    public void TriggerShake(float amplitude, float frequency, float duration)
	{
		isShaking = true;
	    isContinuous = false;
		shakeAmplitude = amplitude;
		shakeDuration = duration;
		shakeCooldown = shakeDuration;
		shakeFrequency = frequency;
	}

    public void TriggerShake(float amplitude, float frequency)
    {
        isShaking = true;
        isContinuous = true;
        isShakingTriggered = true;
        shakeAmplitude = amplitude;
        shakeFrequency = frequency;
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
