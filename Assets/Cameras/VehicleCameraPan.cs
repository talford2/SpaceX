using UnityEngine;

public class VehicleCameraPan : VehicleCameraState
{
    public float PanMaxDistance = 300f;
    public float PanMinDistance = 20f;

    private Vector3 _initPos;
    private float _panDistance;
    private float _panSpeed = 150f;
    private float _turnSpeed = 1f;
    private Vector3 _panDir;

    public VehicleCameraPan(VehicleCamera vehicleCamera) : base(vehicleCamera) { }

    public override void Initialize()
    {
        VehicleCamera.Target = Player.Current.VehicleInstance;
        _initPos = VehicleCamera.Target.transform.position;
        _panDistance = PanMaxDistance;
        _panDir = new Vector3(-1f, 0f, 4f).normalized;
        VehicleCamera.transform.position = _initPos + _panDistance * _panDir.normalized;
        VehicleCamera.transform.LookAt(VehicleCamera.Target.transform, VehicleCamera.Target.transform.up);
        VehicleCamera.Shiftable.ApplyWorldPosition();
    }

    public override void Move(float timeStep)
    {
        _panDistance = Mathf.MoveTowards(_panDistance, PanMinDistance, _panSpeed * timeStep);
        _panDir = Vector3.MoveTowards(_panDir, new Vector3(0f, VehicleCamera.VerticalDistance, -VehicleCamera.DistanceBehind).normalized, _turnSpeed * timeStep).normalized;
        var offset = _panDistance * _panDir.normalized;
        var displacement = VehicleCamera.Target.PrimaryWeaponInstance.GetShootPointCentre() + offset - VehicleCamera.transform.position;
        VehicleCamera.transform.LookAt(VehicleCamera.Target.transform.position, VehicleCamera.Target.transform.up);
        VehicleCamera.Shiftable.Translate(displacement * timeStep);
    }

    public override void Reset()
    {
    }
}
