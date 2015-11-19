using UnityEngine;

public class FighterAttack :NpcState<Fighter>
{
    public FighterAttack(Fighter npc) : base(npc)
    {
        Name = "Attack";
    }

    public override void Update()
    {
        if (Npc.Target == null)
        {
            Npc.VehicleInstance.CurrentWeapon.IsTriggered = false;
            Npc.State = new FighterIdle(Npc);
            return;
        }
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
            if (Mathf.Abs(angleToTarget) < Npc.ShootAngleTolerance)
            {
                Npc.VehicleInstance.SetAimAt(Npc.Target.position);
                Npc.VehicleInstance.CurrentWeapon.IsTriggered = true;
            }
            else
            {
                Npc.VehicleInstance.CurrentWeapon.IsTriggered = false;
            }

            if (toTarget.sqrMagnitude < Npc.OvertakeDistance * Npc.OvertakeDistance)
            {
                Debug.Log("OVERTAKE!");
                Npc.VehicleInstance.TriggerAccelerate = true;
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
