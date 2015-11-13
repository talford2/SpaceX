using System.Collections.Generic;
using UnityEngine;

public class VehicleCamera : UniverseCamera {
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

    private Camera attachedCamera;
    private float springDistance;
    private Quaternion offsetAngle;
    private Vector3 offset;

    private float targetFov;

    private void Start()
    {
        attachedCamera = GetComponent<Camera>();
        springDistance = 1f;
        Target = PlayerController.Current.VehicleInstance;
        offsetAngle = Target.transform.rotation;
    }

    public override void Move()
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
        offset = Vector3.Lerp(offset, offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * springDistance, 2.5f*Time.deltaTime);

        _shiftable.Translate(Target.transform.position + offset - transform.position);

        transform.LookAt(Target.GetAimPosition(), Target.transform.up);

        BackgroundTransform.transform.position = transform.position;

        attachedCamera.fieldOfView = Mathf.Lerp(attachedCamera.fieldOfView, targetFov, Time.deltaTime);

        foreach (var childCam in ChildCameras)
        {
            childCam.fieldOfView = attachedCamera.fieldOfView;
        }
    }
}
