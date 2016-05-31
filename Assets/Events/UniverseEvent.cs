using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Shiftable))]
public class UniverseEvent : MonoBehaviour
{
	public static List<UniverseEvent> UniverseEvents { get; set; }

	public Shiftable Shiftable { get; set; }

    public float TriggerRadius;

    [Header("Tracker")]
    public bool UseTracker;
    public float TrackerRadius = 1000f;
    public Texture2D TrackerCursorImage;
    public Texture2D ArrowCursorImage;
    public Font LabelFont;
    public bool HasBeenTriggered = false;

    private float _triggerRadiusSquared;
    private float _lastDistanceSquared;
    private float _trackerRadiusSquared;

    private EventTracker _tracker;

    private int _cellRadius;
    private CellIndex _universeCellIndex;

	public virtual void Awake()
	{
		if (UniverseEvents == null)
		{
			UniverseEvents = new List<UniverseEvent>();
		}

		Shiftable = GetComponent<Shiftable>();
		Shiftable.OnShift += Shiftable_OnShift;

	    _triggerRadiusSquared = TriggerRadius*TriggerRadius;
	    _trackerRadiusSquared = TrackerRadius*TrackerRadius;

		UniverseEvents.Add(this);

	    _cellRadius = Mathf.CeilToInt(TriggerRadius/Universe.Current.CellSize);
        _universeCellIndex = Universe.Current.ViewPort.Shiftable.UniverseCellIndex;
	}

    private bool InRange(Shiftable sender)
    {
        _universeCellIndex = Universe.Current.ViewPort.Shiftable.UniverseCellIndex;
        var dX = sender.UniverseCellIndex.X - _universeCellIndex.X;
        if (dX >= -_cellRadius && dX <= _cellRadius)
        {
            var dY = sender.UniverseCellIndex.Y - _universeCellIndex.Y;
            if (dY >= -_cellRadius && dY <= _cellRadius)
            {
                var dZ = sender.UniverseCellIndex.Z - _universeCellIndex.Z;
                if (dZ >= -_cellRadius && dZ <= _cellRadius)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Shiftable_OnShift(Shiftable sender, Vector3 delta)
    {
        var thisEvent = sender.GetComponent<UniverseEvent>();
        var enable = false;
        if (!thisEvent.HasBeenTriggered)
        {
            enable = InRange(sender);
        }
        thisEvent.enabled = enable;
    }

    private void Update()
    {
        var toViewPortSquared = (Universe.Current.ViewPort.Shiftable.GetAbsoluteUniversePosition() - Shiftable.GetAbsoluteUniversePosition()).sqrMagnitude;
        if (_lastDistanceSquared > _triggerRadiusSquared)
        {
            if (toViewPortSquared < _triggerRadiusSquared)
            {
                Trigger();
            }
        }
        if (UseTracker)
        {
            if (_tracker == null)
            {
                if (_lastDistanceSquared > _trackerRadiusSquared)
                {
                    if (toViewPortSquared < _trackerRadiusSquared)
                    {
                        _tracker = gameObject.AddComponent<EventTracker>();
                        _tracker.ArrowCursorImage = ArrowCursorImage;
                        _tracker.TrackerCursorImage = TrackerCursorImage;
                        _tracker.LabelFont = LabelFont;
                    }
                }
            }
            if (_tracker != null)
            {
                if (_lastDistanceSquared < _trackerRadiusSquared)
                {
                    if (toViewPortSquared > _trackerRadiusSquared)
                    {
                        _tracker.SelfDestroy();
                    }
                }
            }
        }
        _lastDistanceSquared = toViewPortSquared;
    }

    private void OnDisable()
    {
        if (_tracker != null)
            _tracker.SelfDestroy();
    }

	//private void OnDrawGizmos()
	//{
	//	var children = GetComponentsInChildren<Transform>();
	//	Gizmos.color = Color.red;
	//	foreach (var child in children)
	//	{
	//		Gizmos.DrawLine(transform.position, child.position);
	//	}
	//}

	public virtual void Trigger()
	{
		Debug.Log("Event triggered");
        if (_tracker != null)
            _tracker.SelfDestroy();
	    HasBeenTriggered = true;
	    enabled = false;
	}
}
