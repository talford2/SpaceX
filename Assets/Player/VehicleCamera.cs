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

    [Header("Boosting Effect")]
	public float SpringBoostExpansion = 3f;
    public float BoostFov = 100f;

	[Header("Lerp Speeds")]
	public float RotationCatchup = 3f;
	public float SpringCatchup = 2f;
	public float OffsetCatchup = 5f;

	public AnimationCurve ShakeAmplitude = AnimationCurve.Linear(0, 0, 1, 1);

	private float _springDistance;
    private float _targetSpringDistance;
	private Quaternion _offsetAngle;
	private Vector3 _offset;

	private float _targetFov;
	private Vector3 _targetUp;

    // Shake Params
    private bool _isShaking;
    private bool _isContinuous;
    private bool _isShakingTriggered;
    private float _shakeAmplitude;
    private float _shakeDuration;
    private float _shakeCooldown;
    private float _shakeFrequency;

    private float _amplitudeFrac;
    private float _shakeTime;

    private Vector3 _momentVelocity;

    private void Start()
	{
		_springDistance = 1f;
		Target = PlayerController.Current.VehicleInstance;
		_offsetAngle = Target.transform.rotation;
		_targetUp = Target.transform.up;
        _isShakingTriggered = false;
	}

	public override void Move()
	{
        //Debug.LogFormat("CAM STATE: Boost: {0}, Accel: {1}, Brake: {2}", Target.IsBoosting, Target.IsAccelerating, Target.IsBraking);
        //Debug.LogFormat("CAM STATE: SPRING: {0:f2} TARGET: {1:f2}", _springDistance, _targetSpringDistance);
        if (Target != null)
        {
            _targetSpringDistance = 1f;
            _targetFov = 60f;
            if (Target.IsBoosting)
            {
                _targetSpringDistance = SpringBoostExpansion;
                _targetFov = BoostFov;
            }
            else
            {
                if (Target.IsAccelerating)
                {
                    _targetSpringDistance = SpringExpansion;
                }
                else
                {
                    if (Target.IsBraking)
                    {
                        _targetSpringDistance = SpringCompression;
                    }
                }
            }

            _springDistance = Mathf.Lerp(_springDistance, _targetSpringDistance, SpringCatchup * Time.deltaTime);

            _offsetAngle = Quaternion.Lerp(_offsetAngle, Target.transform.rotation, RotationCatchup * Time.deltaTime);
            _offset = Vector3.Lerp(_offset, _offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * _springDistance, OffsetCatchup * Time.deltaTime);

            Vector3 displacement;
            if (Target.PrimaryWeaponInstance != null)
            {
                displacement = Target.PrimaryWeaponInstance.GetShootPointCentre() + _offset - transform.position;
            }
            else
            {
                displacement = Target.transform.position + _offset - transform.position;
            }
            _shiftable.Translate(displacement);
            _momentVelocity = displacement;

            _targetUp = Vector3.Lerp(_targetUp, Target.transform.up, 5f * Time.deltaTime);

            transform.LookAt(Target.GetAimPosition(), _targetUp);
        }
        else
        {
            _momentVelocity = Vector3.Lerp(_momentVelocity, Vector3.zero, Time.deltaTime);
            _shiftable.Translate(_momentVelocity);
        }

	    BackgroundTransform.transform.position = transform.position;

		AttachedCamera.fieldOfView = Mathf.Lerp(AttachedCamera.fieldOfView, _targetFov, Time.deltaTime);

		foreach (var childCam in ChildCameras)
		{
			childCam.fieldOfView = AttachedCamera.fieldOfView;
		}

		if (_isShaking)
		{
		    if (!_isContinuous)
		    {
		        if (_shakeCooldown >= 0f)
		        {
		            _shakeCooldown -= Time.deltaTime;
                    if (_shakeCooldown < 0f)
                    {
                        AttachedCamera.transform.localPosition = Vector3.zero;
                        _isShaking = true;
                    }

                    var frac = 1f - _shakeCooldown/_shakeDuration;
                    _amplitudeFrac = ShakeAmplitude.Evaluate(frac);
                    var freq = _shakeCooldown*Mathf.Rad2Deg* _shakeFrequency;

                    Shake(_amplitudeFrac, freq);
                }
            }
            else
		    {
		        if (_isShakingTriggered)
		        {
		            _amplitudeFrac = Mathf.Lerp(_amplitudeFrac, 1f, 5f*Time.deltaTime);
		        }
		        else
		        {
                    _amplitudeFrac = Mathf.Lerp(_amplitudeFrac, 0f, 5f * Time.deltaTime);
                }
		        if (_amplitudeFrac < 0.0001f)
		        {
                    AttachedCamera.transform.localPosition = Vector3.zero;
                    _isShaking = false;
		        }

		        _shakeTime += Time.deltaTime;
		        _shakeTime -= Mathf.Floor(_shakeTime);
                var freq = _shakeTime * Mathf.Rad2Deg * _shakeFrequency;

                Shake(_amplitudeFrac, freq);

		        _isShakingTriggered = false;
            }
		}
	}

    private void Shake(float amplitudeFraction, float frequency)
    {
        var x = Mathf.Sin(frequency);
        var y = Mathf.Sin(frequency*1.2f);
        var z = Mathf.Sin(frequency*1.4f);

        var fracVec = new Vector3(x, y, z)*amplitudeFraction;

        AttachedCamera.transform.localPosition = _shakeAmplitude*fracVec;
        AttachedCamera.transform.localRotation = Quaternion.Euler(2f*Mathf.PI*_shakeAmplitude*fracVec);
    }

    public void TriggerShake(float amplitude, float frequency, float duration)
	{
		_isShaking = true;
	    _isContinuous = false;
		_shakeAmplitude = amplitude;
		_shakeDuration = duration;
		_shakeCooldown = _shakeDuration;
		_shakeFrequency = frequency;
	}

    public void TriggerShake(float amplitude, float frequency)
    {
        _isShaking = true;
        _isContinuous = true;
        _isShakingTriggered = true;
        _shakeAmplitude = amplitude;
        _shakeFrequency = frequency;
    }

    public void Reset()
    {
        _springDistance = 1f;

        _offsetAngle = Target.transform.rotation;
        _targetUp = Target.transform.up;
        _offset = _offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * _springDistance;

        transform.position = Target.transform.position + _offset;
        transform.LookAt(Target.transform, Target.transform.up);
        //Debug.Break();
    }
}
