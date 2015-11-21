using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
    private float targetSearchInterval = 3f;
    private float targetSearchCooldown;

    private float neighborDetectInterval = 0.2f;
    private float neighborDetectCooldown;
    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
    }

    public override void Update()
    {
        Npc.VehicleInstance.PitchThotttle = 1f*Time.deltaTime;

        if (neighborDetectCooldown >= 0f)
        {
            neighborDetectCooldown -= Time.deltaTime;
            if (neighborDetectCooldown < 0f)
            {
                Npc.ProximitySensor.Detect(DetectNeighbor);
                neighborDetectCooldown = neighborDetectInterval;
            }
        }

        if (targetSearchCooldown >= 0f)
        {
            targetSearchCooldown -= Time.deltaTime;
            if (targetSearchCooldown < 0f)
            {
                Npc.Target = Targeting.FindFacingAngle(Targeting.GetEnemyTeam(Npc.Team), Npc.VehicleInstance.transform.position, Npc.VehicleInstance.transform.forward, Npc.MaxTargetDistance);
                targetSearchCooldown = targetSearchInterval;
                if (Npc.Target != null)
                {
                    Npc.State = new FighterChase(Npc);
                }
            }
        }
    }

    private void DetectNeighbor(Transform neighbor)
    {
        Debug.Log("NEIGHBOR DETECTED!");
    }
}
