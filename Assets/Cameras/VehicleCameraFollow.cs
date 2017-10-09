using UnityEngine;

public class VehicleCameraFollow : VehicleCameraState
{
    [Header("Offset")]

    [Header("Spring")]
    public float SpringCompression = 0.7f;
    public float SpringExpansion = 1.5f;

    [Header("Boosting Effect")]
    public float SpringBoostExpansion = 1.8f;
    public float BoostFov = 100f;

    [Header("Lerp Speeds")]
    public float RotationCatchup = 5f;
    public float SpringCatchup = 2f;
    public float OffsetCatchup = 5f;

    // Privates
    private float _springDistance;
    private float _targetSpringDistance;
    private Quaternion _offsetAngle;
    private Vector3 _offset;

    private float _targetFov;
    private Vector3 _targetUp;

    private Vector3 _momentVelocity;

    public VehicleCameraFollow(VehicleCamera vehicleCamera) : base(vehicleCamera) { }

    private float _lookAtDelay = 1f;
    private float _lookAtCooldown;

    public override void Initialize()
    {
        VehicleCamera.Target = Player.Current.VehicleInstance;
        _springDistance = 1f;
        _offsetAngle = VehicleCamera.Target.transform.rotation;
        _targetUp = VehicleCamera.Target.transform.up;

        _offset = VehicleCamera.transform.position - VehicleCamera.Target.transform.position;
        _lookAtCooldown = _lookAtDelay;
    }

    public override void Move(float timeStep)
    {
        //Debug.LogFormat("CAM STATE: Boost: {0}, Accel: {1}, Brake: {2}", Target.IsBoosting, Target.IsAccelerating, Target.IsBraking);
        //Debug.LogFormat("CAM STATE: SPRING: {0:f2} TARGET: {1:f2}", _springDistance, _targetSpringDistance);
        if (VehicleCamera.Target != null)
        {
            _targetSpringDistance = 1f;
            _targetFov = 60f;
            if (VehicleCamera.Target.IsBoosting)
            {
                _targetSpringDistance = SpringBoostExpansion;
                _targetFov = BoostFov;
            }
            else
            {
                if (VehicleCamera.Target.IsAccelerating)
                {
                    _targetSpringDistance = SpringExpansion;
                }
                else
                {
                    if (VehicleCamera.Target.IsBraking)
                    {
                        _targetSpringDistance = SpringCompression;
                    }
                }
            }

            _springDistance = Mathf.Lerp(_springDistance, _targetSpringDistance, SpringCatchup * timeStep);

            _offsetAngle = Quaternion.Lerp(_offsetAngle, VehicleCamera.Target.transform.rotation, RotationCatchup * timeStep);
            _offset = Vector3.Lerp(_offset, _offsetAngle * new Vector3(0f, VehicleCamera.VerticalDistance, -VehicleCamera.DistanceBehind) * _springDistance, OffsetCatchup * timeStep);

            Vector3 displacement;
            if (VehicleCamera.Target.PrimaryWeaponInstance != null)
            {
                displacement = VehicleCamera.Target.PrimaryWeaponInstance.GetShootPointCentre() + _offset - VehicleCamera.transform.position;
            }
            else
            {
                displacement = VehicleCamera.Target.transform.position + _offset - VehicleCamera.transform.position;
            }
            VehicleCamera.Shiftable.Translate(displacement);
            _momentVelocity = displacement;

            _targetUp = Vector3.Lerp(_targetUp, VehicleCamera.Target.transform.up, 5f * timeStep);

            if (_lookAtCooldown >= 0f)
            {
                _lookAtCooldown -= Time.deltaTime;
                var _lookAtFraction = 1f - _lookAtCooldown / _lookAtDelay;
                VehicleCamera.transform.rotation = Quaternion.Lerp(VehicleCamera.transform.rotation, Quaternion.LookRotation(VehicleCamera.Target.GetAimPosition() - VehicleCamera.transform.position, _targetUp), _lookAtFraction);
            }
            else
            {
                VehicleCamera.transform.LookAt(VehicleCamera.Target.GetAimPosition(), _targetUp);
            }
        }
        else
        {
            _momentVelocity = Vector3.Lerp(_momentVelocity, Vector3.zero, timeStep);
            VehicleCamera.Shiftable.Translate(_momentVelocity);
        }

        VehicleCamera.AttachedCamera.fieldOfView = Mathf.Lerp(VehicleCamera.AttachedCamera.fieldOfView, _targetFov, timeStep);

        foreach (var childCam in VehicleCamera.ChildCameras)
        {
            childCam.fieldOfView = VehicleCamera.AttachedCamera.fieldOfView;
        }
    }

    public override void Reset()
    {
        _springDistance = 1f;

        _offsetAngle = VehicleCamera.Target.transform.rotation;
        _targetUp = VehicleCamera.Target.transform.up;
        _offset = _offsetAngle * new Vector3(0f, VehicleCamera.VerticalDistance, -VehicleCamera.DistanceBehind) * _springDistance;

        VehicleCamera.transform.position = VehicleCamera.Target.transform.position + _offset;
        VehicleCamera.transform.LookAt(VehicleCamera.Target.transform, VehicleCamera.Target.transform.up);
        //Debug.Break();
    }
}
