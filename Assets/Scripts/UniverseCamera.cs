using UnityEngine;

public abstract class UniverseCamera : MonoBehaviour
{
    public abstract void Move();

    public delegate void OnMoveEvent();
    public event OnMoveEvent OnMove;

    protected Shiftable _shiftable;
    protected Camera _camera;

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    public Camera AttachedCamera
    {
        get { return _camera; }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Move();
        if (OnMove != null)
            OnMove();
    }
}
