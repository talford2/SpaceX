using UnityEngine;
using System.Collections;

public class DistanceGizmo : MonoBehaviour
{
	public float Distance;

	void OnDrawGizmos()
	{
		Distance = (Camera.current.transform.position - transform.position).magnitude;
	}
}