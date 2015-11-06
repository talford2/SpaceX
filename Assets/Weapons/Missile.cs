using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
	private LineRenderer _lineRenderer;

	private Shiftable _shiftable;

	public float MissileLength = 6f;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_shiftable = GetComponent<Shiftable>();
		_shiftable.OnShift += _shiftable_OnShift;
	}

	private void _shiftable_OnShift(CellIndex delta)
	{
		UpdateLineRenderer();
		throw new MissingReferenceException("Fucked!");

	}

	void Update()
	{
		UpdateLineRenderer();
	}

	public void UpdateLineRenderer()
	{
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, transform.position + transform.forward * MissileLength);
	}
}
