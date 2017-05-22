using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Trail : MonoBehaviour
{
    public float Interval = 0.05f;
    public int VertextCount = 10;
    public Shiftable Shiftable;

    private float _trailTime;

    private bool _isVisible;
    private LineRenderer _lineRenderer;
    private List<Vector3> _curPositions;
    private List<Vector3> _oldPositions;

    private void Awake()
    {
        Shiftable.OnShift += Shift;
        _lineRenderer = GetComponent<LineRenderer>();
        Stop();
    }

    private void Update()
    {
        if (_isVisible)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _curPositions[0] = transform.position;
            if (_trailTime > 0f)
            {
                _trailTime -= Time.deltaTime;
                if (_trailTime < 0f)
                {
                    if (_curPositions.Count < VertextCount)
                    {
                        var lastPoint = _curPositions[_curPositions.Count - 1];
                        _curPositions.Add(lastPoint);
                        _oldPositions.Add(lastPoint);
                        _lineRenderer.positionCount = _curPositions.Count;
                    }
                    for (var i = 0; i < _curPositions.Count; i++)
                    {
                        _oldPositions[i] = _curPositions[i];
                    }
                    _curPositions[1] = _curPositions[0];
                    for (var i = 2; i < _curPositions.Count; i++)
                    {
                        _curPositions[i] = _oldPositions[i - 1];
                    }
                    _trailTime = Interval;
                    _lineRenderer.SetPositions(_curPositions.ToArray());
                }
            }
            _lineRenderer.SetPosition(_curPositions.Count - 1, Vector3.Lerp(_oldPositions[_curPositions.Count - 2], _oldPositions[_curPositions.Count - 1], Mathf.Clamp01(_trailTime / Interval)));
        }
    }

    public void Reset()
    {
        _trailTime = Interval;
        _curPositions = new List<Vector3> { transform.position, transform.position };
        _oldPositions = new List<Vector3> { transform.position, transform.position, transform.position };
        _isVisible = true;
        _lineRenderer.positionCount = _curPositions.Count;
        _lineRenderer.enabled = true;
        _lineRenderer.SetPositions(_curPositions.ToArray());
    }

    public void Stop()
    {
        _lineRenderer.enabled = false;
        _isVisible = false;
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
        if (_curPositions != null)
        {
            for (var i = 0; i < _curPositions.Count; i++)
            {
                _curPositions[i] -= delta;
                _lineRenderer.SetPosition(i, _curPositions[i]);
                _oldPositions[i] -= delta;
            }
        }
    }
}
