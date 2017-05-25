using UnityEngine;

public abstract class UniverseCamera : MonoBehaviour
{
    public Camera AttachedCamera;
    public abstract void Move();

    public delegate void OnMoveEvent();
    public event OnMoveEvent OnMove;

    protected Shiftable _shiftable;

    private bool isFree;

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    public void Initialize()
    {
        _shiftable = GetComponent<Shiftable>();
        SetFree(false);
    }

    private void Awake()
    {
        Initialize();
    }

    private void LateUpdate()
    {
        if (!isFree)
        {
            Move();
            if (OnMove != null)
                OnMove();
        }
    }

    public void SetFree(bool value)
    {
        isFree = value;
    }
}
