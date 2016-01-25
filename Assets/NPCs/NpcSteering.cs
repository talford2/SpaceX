public class NpcSteering<T> {
    private T _npc;

    public T Npc
    {
        get { return _npc; }
    }

    protected NpcSteering(T npc)
    {
        _npc = npc;
    }
}
