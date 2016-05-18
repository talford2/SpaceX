using UnityEngine;

public abstract class Npc<T> : MonoBehaviour
{
	private NpcState<T> _state;

	public void UpdateState()
	{
		_state.Update();
	}

	public void SetState(NpcState<T> state)
	{
		_state = state;
		_state.Initialize();
	}
}
