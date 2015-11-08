public class Fighter1 : Npc<Fighter1>
{
    private void Awake()
    {
        State = new Fighter1Idle(this);
    }

    private void Update()
    {
        UpdateState();
    }
}
