using UnityEngine;

public class Fighter1Chase : NpcState<Fighter1>
{
    public Fighter1Chase(Fighter1 npc) : base(npc)
    {
    }

    public override void Update()
    {
        var toTarget = PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition() -  Npc.VehicleInstance.Shiftable.GetWorldPosition();
        var dotTarget = Vector3.Dot(toTarget, Npc.VehicleInstance.transform.forward);

        var pitchYaw = GetPitchYawToPoint(PlayerController.Current.VehicleInstance.transform.position);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x;

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
            Npc.VehicleInstance.TriggerAccelerate = false;
            if (toTarget.sqrMagnitude > 10f * 10f)
            {
                Npc.VehicleInstance.TriggerBrake = true;
            }
            else
            {
                Npc.VehicleInstance.TriggerBrake = false;
            }
        }
    }

    private Vector2 GetPitchYawToPoint(Vector3 point)
    {
        var toPoint = point - Npc.VehicleInstance.transform.position;
        var yawDiff = Vector3.Dot(toPoint, Npc.VehicleInstance.transform.right);
        var pitchDiff = Vector3.Dot(toPoint, Npc.VehicleInstance.transform.up);

        var yawTolerance = 2f;
        var pitchTolerance = 2f;

        var yawAmount = 0f;
        var pitchAmount = 0f;

        if (yawDiff < -yawTolerance)
        {
            yawAmount = Mathf.Clamp(Mathf.Abs(yawDiff)/yawTolerance, 0f, 1f);
        }
        else if (yawDiff > yawTolerance)
        {
            yawAmount = -Mathf.Clamp(Mathf.Abs(yawDiff)/yawTolerance, 0f, 1f);
        }

        if (pitchDiff < -pitchTolerance)
        {
            pitchAmount = Mathf.Clamp(Mathf.Abs(pitchDiff)/pitchTolerance, 0f, 1f);
        }
        else if (pitchDiff > pitchTolerance)
        {
            pitchAmount = -Mathf.Clamp(Mathf.Abs(pitchDiff)/pitchTolerance, 0f, 1f);
        }

        return new Vector2(pitchAmount, -yawAmount);
    }
}
