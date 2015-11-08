using UnityEngine;

public class Fighter1Chase : NpcState<Fighter1>
{
    public Fighter1Chase(Fighter1 npc) : base(npc)
    {
    }

    public override void Update()
    {
        var pitchYaw = GetPitchYawToPoint(PlayerController.Current.VehicleInstance.transform.position);
        Npc.VehicleInstance.YawThrottle = pitchYaw.y;
        Npc.VehicleInstance.PitchThotttle = pitchYaw.x;
    }

    private Vector2 GetPitchYawToPoint(Vector3 point)
    {
        var toPlayer = point - Npc.VehicleInstance.transform.position;
        var yawDiff = Vector3.Dot(toPlayer, Npc.VehicleInstance.transform.right);
        var pitchDiff = Vector3.Dot(toPlayer, Npc.VehicleInstance.transform.up);

        var yawTolerance = 2f;
        var pitchTolerance = 2f;

        var yawAmount = 0f;
        var pitchAmount = 0f;

        if (yawDiff < -yawTolerance)
        {
            yawAmount = Mathf.Clamp(Mathf.Abs(yawDiff) / yawTolerance, 0f, 1f);
        }
        else if (yawDiff > yawTolerance)
        {
            yawAmount = -Mathf.Clamp(Mathf.Abs(yawDiff) / yawTolerance, 0f, 1f);
        }

        if (pitchDiff < -pitchTolerance)
        {
            pitchAmount = Mathf.Clamp(Mathf.Abs(pitchDiff) / pitchTolerance, 0f, 1f);
        }
        else if (pitchDiff > pitchTolerance)
        {
            pitchAmount = -Mathf.Clamp(Mathf.Abs(pitchDiff) / pitchTolerance, 0f, 1f);
        }

        return new Vector2(pitchAmount, -yawAmount);
    }
}
