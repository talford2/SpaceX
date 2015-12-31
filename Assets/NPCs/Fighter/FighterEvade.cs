using System.Collections.Generic;
using UnityEngine;

public class FighterEvade : NpcState<Fighter>
{
    private float evadeTimeout = 2f;
    private float evadeCooldown;

    // Dodge
    private float dodgeCooldown;
    private Vector3 dodgeOffset;

    // Neighbors
    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    private List<Transform> neighbors;

    // Target Reconsider
    private float reconsiderTargetCooldown;
    private float reconsiderTargetInterval = 3f;

    public FighterEvade(Fighter npc) : base(npc)
    {
        Name = "Evade";
        reconsiderTargetCooldown = reconsiderTargetInterval;
    }

    private Quaternion GetRandomArc(float angle)
    {
        var halfAngle = angle*0.5f;
        return Quaternion.Euler(Random.Range(-halfAngle, halfAngle), Random.Range(-halfAngle, halfAngle), Random.Range(-halfAngle, halfAngle));
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

    public Vector3 GetSteerForce(Vector3 targetDestination)
    {
        var steerForce = Vector3.zero;

        steerForce += 0.8f * Npc.Steering.GetSeparationForce(neighbors);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        steerForce += 0.2f * Npc.Steering.GetSeekForce(targetDestination);
        if (steerForce.sqrMagnitude > 1f)
            return steerForce.normalized;

        return steerForce.normalized;
    }

    public override void Update()
    {
        if (Npc.Target == null)
        {
            Npc.State = new FighterIdle(Npc);
            return;
        }
        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;

        if (dodgeCooldown >= 0f)
        {
            dodgeCooldown -= Time.deltaTime;
            if (dodgeCooldown < 0f)
            {
                dodgeOffset = GetRandomArc(Npc.DodgeArcAngle)*-toTarget.normalized*Npc.DodgeRadius;
                dodgeCooldown = Random.Range(Npc.MinDodgeIntervalTime, Npc.MaxDodgeIntervalTime);
            }
        }

        CheckSensors();

        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        var targetDestination = Npc.Target.position - toTarget.normalized*Npc.TurnAroundDistance + dodgeOffset;
        Npc.Destination = GetSteerForce(targetDestination);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;

        if (toTarget.sqrMagnitude < Npc.AcclerateDistance*Npc.AcclerateDistance)
        {
            Npc.VehicleInstance.TriggerAccelerate = true;
        }
        else
        {
            Npc.VehicleInstance.TriggerAccelerate = false;
        }

        if (dotTarget < 0f)
        {
            if (dotTarget < -Npc.TurnAroundDistance)
            {
                //Debug.Log("TURN AROUND!");
                //Debug.Log("DOT: " + dotTarget);
                Npc.State = new FighterChase(Npc);
                return;
            }
            evadeCooldown = evadeTimeout;

            var dotTargetFacing = Vector3.Dot(toTarget, Npc.Target.forward);
            if (dotTargetFacing > 0f)
            {
                // Target isn't looking at me!
                Npc.State = new FighterChase(Npc);
                return;
            }
        }

        if (evadeCooldown > 0f)
        {
            evadeCooldown -= Time.deltaTime;
            if (evadeCooldown < 0f)
            {
                Npc.State = new FighterChase(Npc);
                return;
            }
        }

        // Reconsider Target
        if (reconsiderTargetCooldown >= 0f)
        {
            reconsiderTargetCooldown -= Time.deltaTime;
            if (reconsiderTargetCooldown < 0f)
            {
                Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
                reconsiderTargetCooldown = reconsiderTargetInterval;
                if (Npc.Target != null)
                {
                    Npc.State = new FighterChase(Npc);
                    return;
                }
                Npc.Target = Targeting.FindFacingAngleTeam(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, -Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
            }
        }
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
