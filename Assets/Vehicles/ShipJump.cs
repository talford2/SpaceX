using UnityEngine;

public class ShipJump : MonoBehaviour
{
    public AnimationCurve JumpSpeedCurve;

    private float jumpTime = 0.5f;
    private float jumpCooldown;
    private float jumpDistance = 500f;

    private float idleSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 jumpFrom;
    private Vector3 originalPosition;
    private Vector3 originalForward;
    private bool hasJumped;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalForward = transform.forward;
        originalPosition = transform.position;

        jumpFrom = originalPosition - originalForward * jumpDistance;

        transform.localScale = 0.001f * originalScale;
        transform.position = jumpFrom;

        jumpCooldown = jumpTime;

        //Debug.Break();
    }

    private void Update()
    {
        if (jumpCooldown >= 0f)
        {
            jumpCooldown -= Time.deltaTime;
            var fraction = Mathf.Clamp01(1f - jumpCooldown / jumpTime);

            transform.localScale = originalScale * JumpSpeedCurve.Evaluate(fraction);
            transform.position = jumpFrom + JumpSpeedCurve.Evaluate(fraction) * originalForward * jumpDistance;

            if (jumpCooldown < 0f)
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;
                hasJumped = true;
            }
        }

        if (hasJumped)
        {
            transform.position += originalForward * idleSpeed * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.forward * jumpDistance, transform.position);
    }
}
