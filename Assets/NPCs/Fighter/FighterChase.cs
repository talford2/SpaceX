using UnityEngine;

public class FighterChase : NpcState<Fighter>
{
	public FighterChase(Fighter npc) : base(npc)
	{
		Name = "Chase";
	}

    public override void Update()
    {
        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);
        //Debug.Log("DOT PROD: " + dotTarget);

        var targetDestination = Npc.Target.position;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime);

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
            else
            {
                if (toTarget.sqrMagnitude < Npc.OvertakeDistance * Npc.OvertakeDistance)
                {
                    Npc.VehicleInstance.TriggerAccelerate = true;
                }
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

}
