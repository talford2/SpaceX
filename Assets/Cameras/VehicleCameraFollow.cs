using UnityEngine;

public class VehicleCameraFollow : VehicleCameraState
{
    [Header("Offset")]
    public float VerticalDistance = 3.5f;

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

    public override void Initialize()
    {
        _springDistance = 1f;
        VehicleCamera.Target = PlayerController.Current.VehicleInstance;
        _offsetAngle = VehicleCamera.Target.transform.rotation;
        _targetUp = VehicleCamera.Target.transform.up;
    }

    public override void Move()
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

            _springDistance = Mathf.Lerp(_springDistance, _targetSpringDistance, SpringCatchup * Time.deltaTime);

            _offsetAngle = Quaternion.Lerp(_offsetAngle, VehicleCamera.Target.transform.rotation, RotationCatchup * Time.deltaTime);
            _offset = Vector3.Lerp(_offset, _offsetAngle * new Vector3(0f, VerticalDistance, -VehicleCamera.DistanceBehind) * _springDistance, OffsetCatchup * Time.deltaTime);

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

            _targetUp = Vector3.Lerp(_targetUp, VehicleCamera.Target.transform.up, 5f * Time.deltaTime);

            VehicleCamera.transform.LookAt(VehicleCamera.Target.GetAimPosition(), _targetUp);
        }
        else
        {
            _momentVelocity = Vector3.Lerp(_momentVelocity, Vector3.zero, Time.deltaTime);
            VehicleCamera.Shiftable.Translate(_momentVelocity);
        }

        VehicleCamera.BackgroundTransform.transform.position = VehicleCamera.transform.position;

        VehicleCamera.AttachedCamera.fieldOfView = Mathf.Lerp(VehicleCamera.AttachedCamera.fieldOfView, _targetFov, Time.deltaTime);

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
        _offset = _offsetAngle * new Vector3(0f, VerticalDistance, -VehicleCamera.DistanceBehind) * _springDistance;

        VehicleCamera.transform.position = VehicleCamera.Target.transform.position + _offset;
        VehicleCamera.transform.LookAt(VehicleCamera.Target.transform, VehicleCamera.Target.transform.up);
        //Debug.Break();
    }
}
