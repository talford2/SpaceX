using UnityEngine;

public class VehicleCamera : UniverseCamera {
    public Vehicle Target;

    public Transform BackgroundTransform;

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

    private float springDistance;

    private Quaternion offsetAngle;

    private void Start()
    {
        springDistance = 1f;
        Target = PlayerController.Current.VehicleInstance;
        offsetAngle = Target.transform.rotation;
    }

    public override void Move()
    {
        var targetSpringDistance = 1f;
        if (Target.IsBoosting)
        {
            targetSpringDistance = SpringBoostExpansion;
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
        var offset = offsetAngle * new Vector3(0f, VerticalDistance, -DistanceBehind) * springDistance;

        _shiftable.Translate(Target.transform.position + offset - transform.position);

        transform.LookAt(Target.GetAimPosition(), Target.transform.up);

        BackgroundTransform.transform.position = transform.position;
    }
}
