public class NpcSteering<T> {
    private static T _npc;

    public static T Npc
    {
        get { return default(T); }
    }

    protected NpcSteering(T npc)
    {
        _npc = default(T);
    }
}
