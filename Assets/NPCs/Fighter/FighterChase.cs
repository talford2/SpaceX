using System.Collections.Generic;
using UnityEngine;

public class FighterChase : NpcState<Fighter>
{
    // Neighbors
    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    private List<Transform> neighbors;

	public FighterChase(Fighter npc) : base(npc)
	{
		Name = "Chase";
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
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);
        //Debug.Log("DOT PROD: " + dotTarget);

        CheckSensors();

        var targetDestination = Npc.Target.position;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime) + Npc.Steering.GetSeparationForce(neighbors);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        if (pitchYaw.sqrMagnitude <= 0f)
        {
            Debug.Log("THIS DOES HAPPEN!");
            // Give random value to resolve zero pitchYaw issue.
            pitchYaw = Random.insideUnitCircle;
        }

        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (dotTarget > 0f)
        {
            if (toTarget.sqrMagnitude > Npc.SightRange*Npc.SightRange)
            {
                Npc.VehicleInstance.TriggerAccelerate = true;
            }
            if (toTarget.sqrMagnitude < Npc.AttackRange*Npc.AttackRange)
            {
                Npc.State = new FighterAttack(Npc);
            }
        }
        else
        {
            if (toTarget.sqrMagnitude < Npc.EvadeDistance*Npc.EvadeDistance)
            {
                Npc.State = new FighterEvade(Npc);
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
