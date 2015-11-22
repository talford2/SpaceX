using System.Collections.Generic;
using UnityEngine;

public class FighterSteering : NpcSteering<Fighter>
{
    public FighterSteering(Fighter npc) : base(npc)
    {
    }

    public Vector3 GetSeparationForce(List<Transform> neighbors)
    {
        var avoidSum = Vector3.zero;
        if (neighbors != null)
        {
            //Debug.Log("NEIGHBORS: " + neighbors.Count);
            foreach (var neighbor in neighbors)
            {
                // Note this doesn't work for neighbors inside your position!
                var fromNeighbor = Npc.VehicleInstance.transform.position - neighbor.position;
                avoidSum += fromNeighbor.normalized/Mathf.Max(fromNeighbor.magnitude, 0.1f);
            }
        }
        return avoidSum;
    }
}
