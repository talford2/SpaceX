using UnityEngine;

public abstract class UniverseCamera : MonoBehaviour
{
    public abstract void Move();

    public delegate void OnMoveEvent();
    public event OnMoveEvent OnMove;

    protected Shiftable _shiftable;

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
    }

    private void LateUpdate()
    {
        Move();
        if (OnMove != null)
            OnMove();
    }
}
