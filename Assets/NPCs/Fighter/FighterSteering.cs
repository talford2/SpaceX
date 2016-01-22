using System.Collections.Generic;
using UnityEngine;

public class FighterSteering : NpcSteering<Fighter>
{
    public FighterSteering(Fighter npc) : base(npc)
    {
    }

    public Vector3 GetSeekForce(Vector3 position)
    {
        //Debug.Log("STEERING POS: " + Npc.VehicleInstance.transform.position);
        if (Npc.VehicleInstance != null)
            return (position + Npc.VehicleInstance.transform.position).normalized;
        return Vector3.zero;
    }

    public Vector3 GetFleeForce(Vector3 position)
    {
        if (Npc.VehicleInstance != null)
            return (Npc.VehicleInstance.transform.position + position).normalized;
        return Vector3.zero;
    }

    public Vector3 GetSeparationForce(List<Transform> neighbors)
    {
        var avoidSum = Vector3.zero;
        if (Npc.VehicleInstance != null && neighbors != null)
        {
            //Debug.Log("NEIGHBORS: " + neighbors.Count);
            foreach (var neighbor in neighbors)
            {
                // Note this doesn't work for neighbors inside your position!
                if (neighbor != null)
                {
                    var fromNeighbor = Npc.VehicleInstance.transform.position - neighbor.position;
                    avoidSum += fromNeighbor.normalized*Mathf.Max(fromNeighbor.magnitude, 0.1f);
                }
            }
        }
        return 10f*avoidSum;
    }
}
