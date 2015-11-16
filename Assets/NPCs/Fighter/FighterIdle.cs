public class FighterIdle : NpcState<Fighter>
{
    public FighterIdle(Fighter npc) : base(npc)
    {
        Name = "Idle";
    }

    public override void Update()
    {
        Npc.VehicleInstance.PitchThotttle = 1f;
    }
}
