using UnityEngine;

public class FighterIdle : NpcState<Fighter>
{
    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
    }

    public override void Update()
    {
        Npc.VehicleInstance.PitchThotttle = 1f*Time.deltaTime;
        if (PlayerController.Current.VehicleInstance != null)
        {
            Npc.Target = PlayerController.Current.VehicleInstance.transform;
            Npc.State = new FighterChase(Npc);
        }
    }
}
