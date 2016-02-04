using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShiftTrail : MonoBehaviour
{
    public int TotalPoints = 20;
    public float DistanceInterval = 5;
    public Shiftable Shiftable;

    private LineRenderer lineRenderer;
    private List<Vector3> lastPositions;
    private float lastIntervalTime;
    private float intervalTime;

    private void Awake()
    {
        Shiftable.OnShift += Shift;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
    }

    public void Initialize(Vector3 position)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        lastPositions = new List<Vector3>
        {
            position,
            position
        };

        lineRenderer.SetVertexCount(lastPositions.Count);
        lineRenderer.enabled = true;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            if ((lastPositions[1] - lastPositions[0]).sqrMagnitude < DistanceInterval*DistanceInterval)
            {
                lastPositions[0] = transform.position;
                intervalTime = Time.time - lastIntervalTime;
                lastIntervalTime = Time.time;
            }
            else
            {
                lastPositions.Insert(0, transform.position);

                if (lastPositions.Count > TotalPoints)
                {
                    lastPositions.RemoveAt(lastPositions.Count - 1);
                }
                else
                {
                    lineRenderer.SetVertexCount(lastPositions.Count);
                }
            }

            for (var i = 0; i < lastPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, lastPositions[i]);
            }
        }
    }

    public void Stop()
    {
        lineRenderer.enabled = false;
    }

    private void Shift(Shiftable sender, Vector3 delta)
    {
        if (lastPositions != null)
        {
            for (var i = 0; i < lastPositions.Count; i++)
            {
                lastPositions[i] += delta;
                lineRenderer.SetPosition(i, lastPositions[i]);
            }
        }
    }
}
