using UnityEngine;

public class FollowCamera : UniverseCamera
{
	public Transform Target;

	public Transform BackgroundTransform;

    [Header("Offset")]
	public float DistanceBehind = 8f;
	public float VerticalDistance = 2f;

    [Header("Spring")]
    public float SpringCompression = 0.5f;
    public float SpringExpansion = 1.5f;

    [Header("Lerp Speeds")]
    public float RotationCatchup = 3f;
    public float SpringCatchup = 2f;
    public float OffsetCatchup = 5f;

    private float _springDistance;
    
    private Vector3 _offset;

	private void Start()
	{
	    _springDistance = 1f;
	    Target = Player.Current.VehicleInstance.transform;
	}

	public override void Move()
	{
        transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, RotationCatchup * Time.deltaTime);

	    var targetSpringDistance = 1f;
	    if (Player.Current.VehicleInstance.IsAccelerating)
	    {
	        targetSpringDistance = SpringExpansion;
	    }
	    else
	    {
	        if (Player.Current.VehicleInstance.IsBraking)
	        {
                targetSpringDistance = SpringCompression;
	        }
	    }

        _springDistance = Mathf.Lerp(_springDistance, targetSpringDistance, SpringCatchup * Time.deltaTime);

	    _offset = Vector3.Lerp(_offset, -_springDistance*(transform.forward*DistanceBehind) + (transform.up*VerticalDistance), OffsetCatchup*Time.deltaTime);

        _shiftable.Translate(Target.position + _offset - transform.position);

		BackgroundTransform.transform.position = transform.position;
	}
}
