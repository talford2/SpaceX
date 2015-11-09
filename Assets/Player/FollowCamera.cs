using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public Transform Target;

	public static FollowCamera Current;

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

    private float springDistance;
    
    private Vector3 offset;

    private Shiftable _shiftable;

    public Shiftable Shiftable { get { return _shiftable; } }
	private void Awake()
	{
		Current = this;
	    springDistance = 1f;
	    _shiftable = GetComponent<Shiftable>();
	}

	private void LateUpdate()
	{
        transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, RotationCatchup * Time.deltaTime);

	    var targetSpringDistance = 1f;
	    if (PlayerController.Current.VehicleInstance.IsAccelerating)
	    {
	        targetSpringDistance = SpringExpansion;
	    }
	    else
	    {
	        if (PlayerController.Current.VehicleInstance.IsBraking)
	        {
                targetSpringDistance = SpringCompression;
	        }
	    }
        springDistance = Mathf.Lerp(springDistance, targetSpringDistance, SpringCatchup * Time.deltaTime);

	    offset = Vector3.Lerp(offset, -springDistance*(transform.forward*DistanceBehind) + (transform.up*VerticalDistance), OffsetCatchup*Time.deltaTime);

        _shiftable.Translate(Target.position + offset - transform.position);

		BackgroundTransform.transform.position = transform.position;
	}
}
