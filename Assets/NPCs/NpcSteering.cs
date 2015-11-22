public class NpcSteering<T> {
    private static T _npc;

    public static T Npc
    {
        get { return _npc; }
    }

    protected NpcSteering(T npc)
    {
        _npc = npc;
    }
}
