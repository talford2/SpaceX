using UnityEngine;

public class FighterEvade : NpcState<Fighter>
{
    private float evadeTimeout = 2f;
    private float evadeCooldown;

    public FighterEvade(Fighter npc) : base(npc)
    {
        Name = "Evade";
    }

    public override void Update()
    {
        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);
        //Debug.Log("DOT PROD: " + dotTarget);

        //var targetDestination = targetVehicle.Shiftable.GetWorldPosition() + targetVehicle.GetVelocity() * Time.deltaTime + targetVehicle.transform.forward * 200f;
        var targetDestination = Npc.Target.position - toTarget.normalized * Npc.TurnAroundDistance;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;

        Debug.Log("TT: " + toTarget.sqrMagnitude + " --- " + (Npc.AcclerateDistance * Npc.AcclerateDistance));

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
                Debug.Log("TURN AROUND!");
                Debug.Log("DOT: " + dotTarget);
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
}
