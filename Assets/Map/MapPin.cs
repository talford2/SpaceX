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

    public Shiftable Shiftable
    {
        get { return _shiftable; }
    }

    private void Awake()
    {
        _shiftable = GetComponent<Shiftable>();
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
        if (state == MapPinState.Active)
        {
            ActiveInstance.SetActive(true);
            InactiveInstance.SetActive(false);

            CurrentInstance = ActiveInstance;
        }
        if (state == MapPinState.Inactive)
        {
            ActiveInstance.SetActive(false);
            InactiveInstance.SetActive(true);

            CurrentInstance = InactiveInstance;
        }
    }

    public enum MapPinState
    {
        Active,
        Inactive
    }
}
