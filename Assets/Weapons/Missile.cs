using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
	private LineRenderer _lineRenderer;

	private Shiftable _shiftable;

	public float MissileLength = 6f;

	public float MissileSpeed = 150f;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_shiftable = GetComponent<Shiftable>();
	}

	public void Update()
	{
		transform.position += transform.forward * MissileSpeed * Time.deltaTime;
	}

	public void LateUpdate()
	{
		UpdateLineRenderer();
	}

	public void UpdateLineRenderer()
	{
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, transform.position + transform.forward * MissileLength);
	}
}
