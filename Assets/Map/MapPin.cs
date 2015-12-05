using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class MapPin : MonoBehaviour
{
    public GameObject ActivePin;
    public GameObject InactivePin;

    [HideInInspector]
    public GameObject ActiveInstance;

    [HideInInspector]
    public GameObject InactiveInstance;

    public GameObject CurrentInstance;

    private Shiftable _shiftable;
    private MapPinState _state;

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
        _state = MapPinState.Active;
    }

    private void Start()
    {
        Map.Current.AddPin(this);
    }

    private void OnDestroy()
    {
        Map.Current.RemovePin(this);
    }

    public void SetPinState(MapPinState state)
    {
        _state = state;
        RenderState();
    }

    public void RenderState()
    {
        if (ActiveInstance != null && InactiveInstance != null)
        {
            if (_state == MapPinState.Active)
            {
                ActiveInstance.SetActive(true);
                InactiveInstance.SetActive(false);

                CurrentInstance = ActiveInstance;
            }
            if (_state == MapPinState.Inactive)
            {
                ActiveInstance.SetActive(false);
                InactiveInstance.SetActive(true);

                CurrentInstance = InactiveInstance;
            }
        }
    }

    public enum MapPinState
    {
        Active,
        Inactive
    }
}
