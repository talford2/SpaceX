﻿using UnityEngine;

public class Missile : MonoBehaviour
{
	private LineRenderer _lineRenderer;

	private Shiftable _shiftable;

	public float MissileLength = 6f;

	public float MissileSpeed = 150f;

    public float MissileDamage = 5f;

	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_shiftable = GetComponent<Shiftable>();
	}

	public void Update()
	{
	    var missileRay = new Ray(transform.position, transform.forward);
        var displacement = MissileSpeed * Time.deltaTime;
	    RaycastHit missileHit;
	    if (Physics.Raycast(missileRay, out missileHit, displacement))
	    {
	        var killable = missileHit.collider.GetComponentInParent<Killable>();
	        if (killable != null)
	        {
	            killable.Damage(MissileDamage);
	            _lineRenderer.enabled = false;
	        }
	    }
        transform.position += transform.forward * displacement;
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
