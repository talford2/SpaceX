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

	public AnimationCurve ShakeAmplitude = AnimationCurve.Linear(0, 0, 1, 1);

	private float _springDistance;
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
	    if (Target != null)
	    {
	        var targetSpringDistance = 1f;
	        _targetFov = 60f;
	        if (Target.IsBoosting)
	        {
	            targetSpringDistance = SpringBoostExpansion;
	            _targetFov = 100f;
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

	        _springDistance = Mathf.Lerp(_springDistance, targetSpringDistance, SpringCatchup*Time.deltaTime);

	        _offsetAngle = Quaternion.Lerp(_offsetAngle, Target.transform.rotation, RotationCatchup*Time.deltaTime);
	        _offset = Vector3.Lerp(_offset, _offsetAngle*new Vector3(0f, VerticalDistance, -DistanceBehind)*_springDistance, OffsetCatchup*Time.deltaTime);

	        if (Target.PrimaryWeaponInstance != null)
	        {
	            _shiftable.Translate(Target.PrimaryWeaponInstance.GetShootPointCentre() + _offset - transform.position);
	        }
	        else
	        {
	            _shiftable.Translate(Target.transform.position + _offset - transform.position);
            }

            _targetUp = Vector3.Lerp(_targetUp, Target.transform.up, 5f*Time.deltaTime);

            transform.LookAt(Target.GetAimPosition(), _targetUp);
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
		_offsetAngle = Target.transform.rotation;
		_springDistance = 1f;
		_offset = _offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * _springDistance;
		_targetUp = Target.transform.up;

		transform.position = Target.transform.position + _offset;
		transform.LookAt(Target.transform, Target.transform.up);
		//Debug.Break();
	}
}
