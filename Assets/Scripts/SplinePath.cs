using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SplinePath : MonoBehaviour
{
	[RangeAttribute(0, 1)]
	public float TrackingPosition = 0f;

	[RangeAttribute(1, 20)]
	public int Iterations = 10;

	public List<Vector3> ControlPoints
	{
		get
		{
			var points = GetComponentsInChildren<Transform>().Where(t => t != transform).Select(t => t.position).ToList();

			// Project the first point
			var pre = (points[0] - points[1]).normalized * 1 + points[0];
			points.Insert(0, pre);

			// project the last point
			var post = (points[points.Count - 1] - points[points.Count - 2]) * 1 + points[points.Count - 2];
			points.Add(post);

			return points;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		var controlPoints = ControlPoints;
		var prevControlPoint = controlPoints.First();
		for (int i = 0; i <= controlPoints.Count - 4; i++)
		{
			Gizmos.DrawWireSphere(controlPoints[i + 1], 0.5f);
			var prev = GetSectionPoint(0, controlPoints.Skip(i).ToArray());

			var iterationStep = 1f / ((float)Iterations);
			for (var j = iterationStep; j < 1f; j += iterationStep)
			{
				var travelPoint = GetSectionPoint(j, controlPoints.Skip(i).ToArray());
				Gizmos.DrawLine(prev, travelPoint);
				prev = travelPoint;
			}
			Gizmos.DrawLine(prev, GetSectionPoint(1f, controlPoints.Skip(i).ToArray()));
		}

		Gizmos.DrawWireSphere(controlPoints.Last(), 0.5f);

		// Draw tracking point
		Gizmos.color = Color.yellow;

		Gizmos.DrawSphere(GetPoint(TrackingPosition), 0.7f);
	}

	private Vector3 GetSectionPoint(float t, Vector3[] p)
	{
		var a = 0.5f * (2f * p[1]);
		var b = 0.5f * (p[2] - p[0]);
		var c = 0.5f * (2f * p[0] - 5f * p[1] + 4f * p[2] - p[3]);
		var d = 0.5f * (-p[0] + 3f * p[1] - 3f * p[2] + p[3]);
		var pos = a + (b * t) + (c * t * t) + (d * t * t * t);
		return pos;
	}

	private float GetLengthApproximation(int divisions)
	{
		var pathLength = 0f;

		var cp = ControlPoints;//.Skip(1).ToList();
		for (int i = 1; i < cp.Count - 2; i++)
		{
			pathLength += GetSectionLength(divisions, new Vector3[] { cp[i - 1], cp[i], cp[i + 1], cp[i + 2] });
		}

		return pathLength;
	}

	private float GetSectionLength(int divisions, Vector3[] p)
	{
		var len = 0f;
		float stepDist = 0f;

		var prevStep = p[1];
		for (int i = 0; i < divisions; i++)
		{
			stepDist += 1f / ((float)divisions);
			var newPos = GetSectionPoint(stepDist, p);
			len += (newPos - prevStep).magnitude;
			prevStep = newPos;
		}
		return len;
	}

	public Vector3 GetPoint(float t)
	{
		var cps = ControlPoints;
		var totalLength = GetLengthApproximation(Iterations);
		var timeLength = totalLength * t;
		var runningLength = 0f;

		for (int i = 1; i < cps.Count - 2; i++)
		{
			var sectionLength = GetSectionLength(Iterations, cps.Skip(i - 1).Take(4).ToArray());

			if ((runningLength + sectionLength) >= timeLength)
			{
				var diff = (timeLength - runningLength) / (sectionLength);
				var pos = GetSectionPoint(diff, new Vector3[] { cps[i - 1], cps[i], cps[i + 1], cps[i + 2] });
				return pos;
			}

			runningLength += sectionLength;
		}

		return GetSectionPoint(1f, new Vector3[] { cps[cps.Count - 4], cps[cps.Count - 3], cps[cps.Count - 2], cps[cps.Count - 1] });
	}
}