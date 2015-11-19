using UnityEngine;
using System.Collections;

public class UniverseEvent : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		var children = GetComponentsInChildren<Transform>();
		Gizmos.color = Color.red;
		foreach (var child in children)
		{
			Gizmos.DrawLine(transform.position, child.position);
		}
	}
}
