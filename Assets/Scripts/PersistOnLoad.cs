using UnityEngine;

public class PersistOnLoad : MonoBehaviour
{
    private static PersistOnLoad _current;

    public static PersistOnLoad Current
    {
        get { return _current; }
    }

    private void Awake()
    {
        if (_current != null && _current != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _current = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
