using UnityEngine;

public class Fighter1Chase : NpcState<Fighter1>
{
    public Fighter1Chase(Fighter1 npc) : base(npc)
    {
        Name = "Chase";
    }

    public override void Update()
    {
        var targetVehicle = PlayerController.Current.VehicleInstance;

        var toTarget = targetVehicle.Shiftable.GetWorldPosition() - Npc.VehicleInstance.Shiftable.GetWorldPosition();
        var dotTarget = Vector3.Dot(toTarget.normalized, Npc.VehicleInstance.transform.forward);
        Debug.Log("DOT PROD: " + dotTarget);

        var pitchYaw = Npc.GetPitchYawToPoint(targetVehicle.transform.position);
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
            /*
            Npc.VehicleInstance.TriggerAccelerate = false;
            if (toTarget.sqrMagnitude > 10f * 10f)
            {
                Npc.VehicleInstance.TriggerBrake = true;
            }
            else
            {
                Npc.VehicleInstance.TriggerBrake = false;
            }
            */
            Npc.State = new Fighter1Evade(Npc);
        }
    }
    
}
