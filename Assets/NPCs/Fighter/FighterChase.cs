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
        var dotTarget = Vector3.Dot(toTarget.normalized, Npc.VehicleInstance.transform.forward);
        Debug.Log("DOT PROD: " + dotTarget);

        var targetDestination = Npc.Target.position;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;

        if (dotTarget < 0f)
        {
            if (toTarget.sqrMagnitude > 50f*50f)
            {
                Npc.VehicleInstance.TriggerAccelerate = true;
            }
            else
            {
                Npc.VehicleInstance.TriggerAccelerate = false;
            }

            if (toTarget.sqrMagnitude < 10f*10f)
            {
                Npc.VehicleInstance.TriggerBrake = true;
            }
            else
            {
                Npc.VehicleInstance.TriggerBrake = false;
            }
        }
        else
        {
            //Npc.State = new FighterEvade(Npc);
        }
    }
    
}
