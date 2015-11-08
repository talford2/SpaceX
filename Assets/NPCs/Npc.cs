using UnityEngine;

public abstract class Npc<T> : MonoBehaviour
{
    public NpcState<T> State;

    public void UpdateState()
    {
        State.Update();
    }
}
