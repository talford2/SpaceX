using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipJump : MonoBehaviour
{
    public AnimationCurve DisplacementCurve;
    public List<TrailRenderer> Trails;
    public ParticleSystem Flash;
    public float Delay = 2f;
    public GameObject SpawnShipPrefab;
    public AudioClip ArriveSound;

    private float jumpTime = 0.4f;
    private float jumpCooldown;
    private float jumpDistance = 500f;

    private float idleSpeed = 10f;

    private Renderer[] meshRenderers;

    private Vector3 originalScale;
    private Vector3 originalForward;
    private bool hasJumped;

    private bool _isTriggered;
    private UniversePosition _originalPosition;
    private Shiftable _shiftable;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalForward = transform.forward;

        _shiftable = GetComponent<Shiftable>();
        _originalPosition = _shiftable.UniversePosition;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        SetRenderersEmabled(false);
        SetTrailsVisible(false);

        StartCoroutine(DelayedJump(Delay));
        //TriggerJump();
    }

    private void Update()
    {
        if (_isTriggered)
        {
            if (jumpCooldown >= 0f)
            {
                jumpCooldown -= Time.deltaTime;
                var fraction = Mathf.Clamp01(1f - jumpCooldown / jumpTime);

                transform.localScale = originalScale * DisplacementCurve.Evaluate(fraction);
                _shiftable.Translate(DisplacementCurve.Evaluate(fraction) * originalForward * jumpDistance);

                if (jumpCooldown < 0f)
                {
                    transform.localScale = originalScale;
                    _shiftable.SetShiftPosition(_originalPosition);
                    hasJumped = true;
                    if (SpawnShipPrefab != null)
                    {
                        SpawnShip();
                    }
                }
            }
        }
    }

    private IEnumerator DelayedJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerJump();
    }

    private void TriggerJump()
    {
       var jumpFrom = Universe.Current.GetWorldPosition(_originalPosition) + originalForward * jumpDistance;

        Flash.transform.position = jumpFrom;
        Flash.Play();

        transform.localScale = 0.001f * originalScale;

        _shiftable.SetShiftPosition(Universe.Current.GetUniversePosition(jumpFrom));

        SetRenderersEmabled(true);
        SetTrailsVisible(true);

        hasJumped = false;
        jumpCooldown = jumpTime;

        _isTriggered = true;
        Debug.Log("JUMP!");
    }

    private void SpawnShip()
    {
        var ship = Instantiate(SpawnShipPrefab, transform.position, transform.rotation);
        var shipShiftable = ship.GetComponent<Shiftable>();
        shipShiftable.SetShiftPosition(_shiftable.UniversePosition);

        if (ArriveSound != null)
            ResourcePoolIndex.PlayAnonymousSound(ArriveSound, transform.position, 50f, 1000f, 10f, true);

        StartCoroutine(DelayedDestroy(2f));
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
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
