using System.Collections.Generic;
using UnityEngine;

public class FighterSteering : NpcSteering<Fighter>
{
	public FighterSteering(Fighter npc) : base(npc)
	{
	}

	public Vector3 GetSeekForce(Vector3 position)
	{
		if (Npc.VehicleInstance != null)
			return (position - Npc.VehicleInstance.transform.position).normalized;
		return Vector3.zero;
	}

	public Vector3 GetFleeForce(Vector3 position)
	{
		if (Npc.VehicleInstance != null)
			return (Npc.VehicleInstance.transform.position - position).normalized;
		return Vector3.zero;
	}

	// Performance variables
	private Vector3 _avoidSum;
	private Vector3 _fromNeighbour;
    private float _distance;

	public Vector3 GetSeparationForce(List<Detectable> neighbours)
	{
		_avoidSum = Vector3.zero;
		if (Npc.VehicleInstance != null && neighbours != null)
		{
			foreach (var neighbour in neighbours)
			{
				// Note this doesn't work for neighbours inside your position!
				if (neighbour != null)
				{
                    _fromNeighbour = Npc.VehicleInstance.transform.position - neighbour.TargetTransform.position;
                    _distance = _fromNeighbour.magnitude - neighbour.Radius;
                    _avoidSum += _fromNeighbour.normalized / Mathf.Max(_distance, 0.1f);
				}
			}
		}
		return 20f * _avoidSum;
	}
}
