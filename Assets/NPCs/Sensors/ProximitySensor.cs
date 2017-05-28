using System;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    public float Radius;

    private int _detectableMask;
    private const int _detectableMaxBuffer = 1024;
    private Collider[] _detectables;
    private Detectable _curDetectable;

    public void Awake()
    {
        _detectableMask = LayerMask.GetMask("Detectable");
        _detectables = new Collider[_detectableMaxBuffer];
    }

    public void Detect(Action<Detectable> action)
    {
        var detectableCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, _detectables, _detectableMask);
        for (var i = 0; i < detectableCount; i++)
        {
            _curDetectable = _detectables[i].GetComponent<Detectable>();
            if (_curDetectable != null)
                action(_curDetectable);
        }
    }
}
