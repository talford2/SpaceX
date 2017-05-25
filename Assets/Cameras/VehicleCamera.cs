using System.Collections.Generic;
using UnityEngine;

public class VehicleCamera : UniverseCamera
{
	public Vehicle Target;

	public Transform BackgroundTransform;
    public float DistanceBehind = 15f;

    public List<Camera> ChildCameras;
    public AnimationCurve ShakeAmplitude = AnimationCurve.Linear(0, 0, 1, 1);

    // States
    public VehicleCameraFollow Follow;

    private VehicleCameraState _state;

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
        Follow = new VehicleCameraFollow(this);
        SetState(Follow);
	}

	public override void Move()
	{
        _state.Move();
        MoveShake();
	}

    public void SetState(VehicleCameraState state)
    {
        _state = state;
        _state.Initialize();
        _isShaking = false;
    }

    public void MoveShake()
    {
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

                    var frac = 1f - _shakeCooldown / _shakeDuration;
                    _amplitudeFrac = ShakeAmplitude.Evaluate(frac);
                    var freq = _shakeCooldown * Mathf.Rad2Deg * _shakeFrequency;

                    Shake(_amplitudeFrac, freq);
                }
            }
            else
            {
                if (_isShakingTriggered)
                {
                    _amplitudeFrac = Mathf.Lerp(_amplitudeFrac, 1f, 5f * Time.deltaTime);
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

    public void Shake(float amplitudeFraction, float frequency)
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
        if (_state != null)
            _state.Reset();
    }
}
