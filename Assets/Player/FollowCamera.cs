using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	public Transform Target;

	public static FollowCamera Current;

	public float LookAtSpeed = 5f;

	public float DistanceBehind = 8f;

	public float VerticalDistance = 2f;

	public float ChaseSpeed = 5f;

	void Awake()
	{
		Current = this;
	}

	void LateUpdate()
	{
		// Chase
		var chasePos = Target.position - (Target.transform.forward * DistanceBehind) + (Target.transform.up * VerticalDistance);
		transform.position = Vector3.Slerp(transform.position, chasePos, ChaseSpeed * Time.deltaTime);

		// Look at
		var targetLookat = Quaternion.LookRotation(Target.position - transform.position, Target.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetLookat, LookAtSpeed * Time.deltaTime);
	}
}
