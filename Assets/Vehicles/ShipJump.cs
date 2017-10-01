using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipJump : MonoBehaviour
{
    public AnimationCurve DisplacementCurve;
    public List<TrailRenderer> Trails;
    public ParticleSystem Flash;

    private float jumpTime = 0.4f;
    private float jumpCooldown;
    private float jumpDistance = 500f;

    private float idleSpeed = 5f;

    private Renderer[] meshRenderers;

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

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        SetRenderersEmabled(false);
        SetTrailsVisible(false);

        StartCoroutine(DelayedJump(2f));
        //TriggerJump();
    }

    private void Update()
    {
        if (jumpCooldown >= 0f)
        {
            jumpCooldown -= Time.deltaTime;
            var fraction = Mathf.Clamp01(1f - jumpCooldown / jumpTime);

            transform.localScale = originalScale * DisplacementCurve.Evaluate(fraction);
            transform.position = jumpFrom + DisplacementCurve.Evaluate(fraction) * originalForward * jumpDistance;

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

    private IEnumerator DelayedJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerJump();
    }

    private void TriggerJump()
    {
        jumpFrom = originalPosition - originalForward * jumpDistance;

        Flash.transform.position = jumpFrom;
        Flash.Play();

        transform.localScale = 0.001f * originalScale;
        transform.position = jumpFrom;

        SetRenderersEmabled(true);
        SetTrailsVisible(true);

        hasJumped = false;
        jumpCooldown = jumpTime;

        //Debug.Break();
    }

    private void SetRenderersEmabled(bool value)
    {
        foreach(var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = value;
        }
    }

    private void SetTrailsVisible(bool value)
    {
        foreach(var trail in Trails)
        {
            trail.enabled = value;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.forward * jumpDistance, transform.position);
    }
}
