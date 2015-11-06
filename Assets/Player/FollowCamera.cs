using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform Target;

    public static FollowCamera Current;

    public float LookAtSpeed = 5f;

    public float DistanceBehind = 8f;

    public float VerticalDistance = 2f;

    public float ChaseSpeed = 5f;

    private void Awake()
    {
        Current = this;
    }

    private void LateUpdate()
    {
        //var chasePos = Target.position - (Target.transform.forward * DistanceBehind) + (Target.transform.up * VerticalDistance);

        transform.forward =  Vector3.Lerp(transform.forward, Target.transform.forward, 5f*Time.deltaTime);
        transform.position = Target.position - (transform.forward * DistanceBehind) + (transform.up * VerticalDistance);

        /*
        // Chase
        var chasePos = Target.position - (Target.transform.forward*DistanceBehind) + (Target.transform.up*VerticalDistance);
        transform.position = Vector3.Slerp(transform.position, chasePos, ChaseSpeed*Time.deltaTime);

        // Look at
        var targetLookat = Quaternion.LookRotation(Target.position - transform.position, Target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLookat, LookAtSpeed*Time.deltaTime);
        */
    }
}
