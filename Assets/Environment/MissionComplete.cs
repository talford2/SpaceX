public class MissionComplete : Triggerable
{
    public float Delay = 5f;

    public override void Trigger(float delay = 0)
    {
        Mission.Current.TriggerFinish(Delay);
    }
}
