using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Shiftable))]
public class UniverseEvent : MonoBehaviour
{
	public static List<UniverseEvent> UniverseEvents { get; set; }

	public Shiftable Shiftable { get; set; }

    public float TriggerRadius;

    [Header("Tracker")]
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
	}

	private void Shiftable_OnShift(Shiftable sender, Vector3 delta)
	{
		//Debug.Log(sender.UniverseCellIndex + " ==? " + Universe.Current.ViewPort.Shiftable.UniverseCellIndex);

	    var enable = false;
	    var thisEvent = sender.GetComponent<UniverseEvent>();
	    for (var i = -_cellRadius; i < _cellRadius + 1; i++)
	    {
	        for (var j = -_cellRadius; j < _cellRadius + 1; j++)
	        {
	            for (var k = -_cellRadius; k < _cellRadius + 1; k++)
	            {
	                if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex + new CellIndex(i, j, k)))
	                {
	                    if (!thisEvent.HasBeenTriggered)
	                    {
	                        enable = true;
	                    }
	                }
	            }
	        }
	    }
        thisEvent.enabled = enable;

        /*
		if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex))
		{
			//Debug.Log("Go!");

			Initialise();
		}
        */
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
