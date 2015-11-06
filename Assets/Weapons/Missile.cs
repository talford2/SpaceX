using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
	private LineRenderer _lineRenderer;

	public float MissileLength = 6f;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, transform.position + transform.forward * MissileLength);
	}
}
