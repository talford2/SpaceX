public abstract class NpcState<T>
{
    public string Name;

    private readonly T _npc;

    public T Npc
    {
        get { return _npc; }
    }

    protected NpcState(T npc)
    {
        _npc = npc;
    }

    public abstract void Update();

	public abstract void Initialize();
}
