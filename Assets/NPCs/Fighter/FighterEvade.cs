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

    public FighterEvade(Fighter npc) : base(npc)
    {
        Name = "Evade";
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
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime) + Npc.Steering.GetSeparationForce(neighbors);

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
            }
            else
            {
                evadeCooldown = evadeTimeout;
            }
        }

        if (evadeCooldown > 0f)
        {
            evadeCooldown -= Time.deltaTime;
            if (evadeCooldown < 0f)
            {
                Npc.State = new FighterChase(Npc);
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
