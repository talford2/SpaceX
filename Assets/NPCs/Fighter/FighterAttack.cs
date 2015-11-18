using UnityEngine;
using System.Collections;

public class FighterAttack :NpcState<Fighter>
{
    public FighterAttack(Fighter npc) : base(npc)
    {
        Name = "Attack";
    }

    public override void Update()
    {
        var toTarget = Npc.Target.position - Npc.VehicleInstance.transform.position;
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        var targetDestination = Npc.Target.position;
        Npc.Destination = Vector3.Lerp(Npc.Destination, targetDestination, Time.deltaTime);

        var pitchYaw = Npc.GetPitchYawToPoint(Npc.Destination);

        Npc.VehicleInstance.YawThrottle = pitchYaw.y * Time.deltaTime;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x * Time.deltaTime;

        Npc.VehicleInstance.TriggerBrake = false;
        Npc.VehicleInstance.TriggerAccelerate = false;

        if (dotTarget > 10f)
        {
            var angleToTarget = Vector3.Angle(toTarget.normalized, Npc.VehicleInstance.transform.forward.normalized);
            if (Mathf.Abs(angleToTarget) < 5f)
            {
                Npc.VehicleInstance.SetAimAt(Npc.Target.position);
                Npc.VehicleInstance.CurrentWeapon.IsTriggered = true;
            }
            else
            {
                Npc.VehicleInstance.CurrentWeapon.IsTriggered = false;
            }

        }
        else
        {
            Npc.VehicleInstance.CurrentWeapon.IsTriggered = false;
            if (dotTarget < 0f)
                Npc.State = new FighterEvade(Npc);
        }
    }
}
