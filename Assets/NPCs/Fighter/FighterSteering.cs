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
	private Vector3 _fromNeighbor;

	public Vector3 GetSeparationForce(List<Transform> neighbors)
	{
		_avoidSum = Vector3.zero;
		if (Npc.VehicleInstance != null && neighbors != null)
		{
			foreach (var neighbor in neighbors)
			{
				// Note this doesn't work for neighbors inside your position!
				if (neighbor != null)
				{
					_fromNeighbor = Npc.VehicleInstance.transform.position - neighbor.position;
					_avoidSum += _fromNeighbor.normalized / Mathf.Max(_fromNeighbor.magnitude, 0.1f);
				}
			}
		}
		return 20f * _avoidSum;
	}
}
