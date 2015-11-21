using System.Collections.Generic;
using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
    private float targetSearchInterval = 3f;
    private float targetSearchCooldown;

    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;

    private List<Transform> neighbors;
    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
    }

    public override void Update()
    {
        if (neighborDetectCooldown >= 0f)
        {
            neighbors = new List<Transform>();
            neighborDetectCooldown -= Time.deltaTime;
            if (neighborDetectCooldown < 0f)
            {
                Npc.ProximitySensor.Detect(DetectNeighbor);
                neighborDetectCooldown = neighborDetectInterval;
            }
        }

        if (targetSearchCooldown >= 0f)
        {
            targetSearchCooldown -= Time.deltaTime;
            if (targetSearchCooldown < 0f)
            {
                Npc.Target = Targeting.FindFacingAngle(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
                targetSearchCooldown = targetSearchInterval;
                if (Npc.Target != null)
                {
                    Npc.State = new FighterChase(Npc);
                }
            }
        }

        // Steering stuff
        var immediateDestination = Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward*5f;
        if (neighbors != null)
        {
            Debug.Log("NEIGHBORS: " + neighbors.Count);
            var avoidSum = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                // Note this doesn't work for neighbors inside your position!
                var fromNeighbor = Npc.VehicleInstance.transform.position - neighbor.position;
                avoidSum += fromNeighbor.normalized/Mathf.Max(fromNeighbor.magnitude, 0.1f);
            }
            immediateDestination += avoidSum;
        }
        Npc.Destination = immediateDestination;
        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y * Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x * Time.deltaTime;
    }

    private void DetectNeighbor(Transform neighbor)
    {
        if (neighbor != Npc.VehicleInstance.transform)
        {
            if (!neighbors.Contains(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
    }
}
