using System;
using UnityEngine;

public class ProximitySensor : MonoBehaviour {

    public float Radius;

    public void Detect(Action<Transform> action)
    {
        var detectables = Physics.OverlapSphere(transform.position, Radius, LayerMask.GetMask("Detectable"));
        foreach (var detected in detectables)
        {
            var detectable = detected.GetComponent<Detectable>();
            if (detectable != null)
                action(detectable.TargetTransform);
        }
    }
}
