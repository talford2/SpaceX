using System.Collections.Generic;
using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
    private float targetSearchInterval = 3f;
    private float targetSearchCooldown;

    // Neighbors
    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    private List<Transform> neighbors;

    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
    }

    private void CheckSensors()
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
    }

    public override void Update()
    {
        CheckSensors();

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
        var immediateDestination = Npc.VehicleInstance.transform.position + Npc.VehicleInstance.transform.forward*5f + Npc.Steering.GetSeparationForce(neighbors);
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
