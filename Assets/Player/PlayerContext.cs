using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    private static PlayerContext _current;
    public static PlayerContext Current { get { return _current; } }

    public int LevelIndex;

    private void Awake()
    {
        _current = this;
        DontDestroyOnLoad(this);
    }
}
