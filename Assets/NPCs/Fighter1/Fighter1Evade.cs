using UnityEngine;

public class Fighter1Evade : NpcState<Fighter1>
{
    private float evadeTimeout = 2f;
    private float evadeCooldown;

    private Vector3 destination;

    public Fighter1Evade(Fighter1 npc) : base(npc)
    {
        Name = "Evade";
    }

    public override void Update()
    {
        var targetVehicle = PlayerController.Current.VehicleInstance;

        var toTarget = targetVehicle.Shiftable.GetWorldPosition() - Npc.VehicleInstance.Shiftable.GetWorldPosition();
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        var targetDestination = Npc.VehicleInstance.Shiftable.GetWorldPosition() + Npc.VehicleInstance.transform.forward*20f;
        if (toTarget.sqrMagnitude < 400f)
        {
            targetDestination = targetVehicle.Shiftable.GetWorldPosition() + targetVehicle.GetVelocity() + targetVehicle.transform.forward * 400f;
        }
        destination = Vector3.Lerp(destination, targetDestination, Time.deltaTime);


        var pitchYaw = Npc.GetPitchYawToPoint(destination);
        Npc.VehicleInstance.YawThrottle = 0.5f*pitchYaw.y * Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = 0.5f*pitchYaw.x * Time.deltaTime;

        var toDestination = destination - Npc.VehicleInstance.transform.position;
        if (toDestination.sqrMagnitude > 50f * 50f)
        {
            Npc.VehicleInstance.TriggerAccelerate = true;
        }
        else
        {
            Npc.VehicleInstance.TriggerAccelerate = false;
        }

        if (dotTarget > 0f)
        {
            evadeCooldown = evadeTimeout;
        }

        if (evadeCooldown > 0f)
        {
            evadeCooldown -= Time.deltaTime;
            if (evadeCooldown < 0f)
            {
                Npc.State = new Fighter1Chase(Npc);
            }
        }
    }
}
