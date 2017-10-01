﻿using UnityEngine;

public class ShipJump : MonoBehaviour
{
    private float jumpTime = 0.5f;
    private float jumpCooldown;
    private float jumpDistance = 500f;

    private Vector3 originalScale;
    private Vector3 jumpFrom;
    private Vector3 originalPosition;
    private Vector3 originalForward;

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

            transform.localScale = originalScale * fraction;
            Debug.Log("FRACTION: " + fraction);
            transform.position = jumpFrom + fraction * originalForward * jumpDistance;
            //fraction * originalForward * jumpDistance;

            if (jumpCooldown < 0f)
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.forward * jumpDistance, transform.position);
    }
}
