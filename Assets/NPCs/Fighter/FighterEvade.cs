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
        var dotTarget = Vector3.Dot(toTarget.normalized, Npc.VehicleInstance.transform.forward);
        Debug.Log("DOT PROD: " + dotTarget);

        //var targetDestination = targetVehicle.Shiftable.GetWorldPosition() + targetVehicle.GetVelocity() * Time.deltaTime + targetVehicle.transform.forward * 200f;
        var targetDestination = Npc.Target.position + Npc.Target.forward * 200f;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y*Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x*Time.deltaTime;

        /*
        var toDestination = Npc.Destination - Npc.VehicleInstance.transform.position;
        if (toDestination.sqrMagnitude > 50f*50f)
        {
            Npc.VehicleInstance.TriggerAccelerate = true;
        }
        else
        {
            Npc.VehicleInstance.TriggerAccelerate = false;
        }
        

        if (targetVehicle.GetVelocity().sqrMagnitude > Npc.VehicleInstance.GetVelocity().sqrMagnitude)
        {
            Npc.VehicleInstance.TriggerBrake = false;
        }
        else
        {
            Npc.VehicleInstance.TriggerBrake = true;

        }
        */

        if (dotTarget > 0f)
        {
            evadeCooldown = evadeTimeout;
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
