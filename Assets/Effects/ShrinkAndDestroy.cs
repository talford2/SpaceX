using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    public float Delay;
    public float ShrinkTime;

    private bool _isTriggered;
    private Transform _transform;
    private float _shrinkCooldown;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        StartCoroutine(DelayedTrigger(Delay));
    }

    private IEnumerator DelayedTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        Trigger();
    }

    private void Trigger()
    {
        _isTriggered = true;
        _shrinkCooldown = ShrinkTime;
    }

    private void Update()
    {
        if (_isTriggered)
        {
            if (_shrinkCooldown >= 0f)
            {
                _shrinkCooldown -= Time.deltaTime;
                var shrinkFraction = Mathf.Clamp01(_shrinkCooldown / ShrinkTime);
                /*
                if (_shrinkCooldown > 0.1f)
                    _transform.localScale *= shrinkFraction;
                */
                if (_shrinkCooldown < 0f)
                    Destroy(gameObject);
            }
        }
    }
}
