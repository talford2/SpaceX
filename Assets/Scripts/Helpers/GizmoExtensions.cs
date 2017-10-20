using UnityEngine;

public class GizmoExtensions {

	public static void AxesPoint(Vector3 position, float radius = 0.5f)
    {
        Gizmos.DrawLine(position - radius * Vector3.up, position + radius * Vector3.up);
        Gizmos.DrawLine(position - radius * Vector3.right, position + radius * Vector3.right);
        Gizmos.DrawLine(position - radius * Vector3.forward, position + radius * Vector3.forward);
    }
}
