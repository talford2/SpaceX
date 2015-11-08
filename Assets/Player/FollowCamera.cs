using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public Transform Target;

	public static FollowCamera Current;

	public Transform BackgroundTransform;

	public float DistanceBehind = 8f;

	public float VerticalDistance = 2f;

    public float SpringCompression = 0.5f;

    public float SpringExpansion = 1.5f;

    private float springDistance;

	private void Awake()
	{
		Current = this;
	    springDistance = 1f;
	}

	private void LateUpdate()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, 5f * Time.deltaTime);

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
        springDistance = Mathf.Lerp(springDistance, targetSpringDistance, 2f * Time.deltaTime);

        transform.position = Target.position - springDistance*(transform.forward * DistanceBehind) + (transform.up * VerticalDistance);

		BackgroundTransform.transform.position = transform.position;
	}
}
