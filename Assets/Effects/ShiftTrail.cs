using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShiftTrail : MonoBehaviour
{
    public int TotalPoints = 20;
    public float DistanceInterval = 5;
    public Shiftable Shiftable;

    private LineRenderer _lineRenderer;
    private List<Vector3> _lastPositions;
    private float _lastIntervalTime;
    private float _intervalTime;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        _lastPositions = new List<Vector3>
        {
            transform.position,
            transform.position
        };
    }

    public void Initialize(Vector3 position)
    {
        Shiftable.OnShift += Shift;

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        _lastPositions = new List<Vector3>
        {
            position,
            position
        };

        _lineRenderer.SetVertexCount(_lastPositions.Count);
        _lineRenderer.enabled = true;
    }

    private void LateUpdate()
    {
        if (_lineRenderer.enabled)
        {
            if ((_lastPositions[1] - _lastPositions[0]).sqrMagnitude < DistanceInterval*DistanceInterval)
            {
                _lastPositions[0] = transform.position;
                _intervalTime = Time.time - _lastIntervalTime;
                _lastIntervalTime = Time.time;
            }
            else
            {
                _lastPositions.Insert(0, transform.position);

                if (_lastPositions.Count > TotalPoints)
                {
                    _lastPositions.RemoveAt(_lastPositions.Count - 1);
                }
                else
                {
                    _lineRenderer.SetVertexCount(_lastPositions.Count);
                }
            }

            for (var i = 0; i < _lastPositions.Count; i++)
            {
                _lineRenderer.SetPosition(i, _lastPositions[i]);
            }
        }
    }

    public void Stop()
    {
        _lineRenderer.enabled = false;
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
        if (_lastPositions != null)
        {
            for (var i = 0; i < _lastPositions.Count; i++)
            {
                _lastPositions[i] -= delta;
                _lineRenderer.SetPosition(i, _lastPositions[i]);
            }
        }
    }
}
