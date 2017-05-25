using UnityEngine;

public abstract class VehicleCameraState
{
    private readonly VehicleCamera _vehicleCamera;

    public VehicleCamera VehicleCamera
    {
        get { return _vehicleCamera; }
    }

    public VehicleCameraState(VehicleCamera vehicleCamera)
    {
        _vehicleCamera = vehicleCamera;
    }

    public abstract void Initialize();

    public abstract void Move(float timeStep);

    public abstract void Reset();
}
