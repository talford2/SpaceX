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

    private float triggerRadiusSquared;
    private float lastDistanceSquared;
    private float trackerRadiusSquared;

    private EventTracker tracker;

    private int cellRadius;

	public virtual void Awake()
	{
		if (UniverseEvents == null)
		{
			UniverseEvents = new List<UniverseEvent>();
		}

		Shiftable = GetComponent<Shiftable>();
		Shiftable.OnShift += Shiftable_OnShift;

	    triggerRadiusSquared = TriggerRadius*TriggerRadius;
	    trackerRadiusSquared = TrackerRadius*TrackerRadius;

		UniverseEvents.Add(this);

	    cellRadius = Mathf.CeilToInt(TriggerRadius/Universe.Current.CellSize);
	}

	private void Shiftable_OnShift(Shiftable sender, Vector3 delta)
	{
		//Debug.Log(sender.UniverseCellIndex + " ==? " + Universe.Current.ViewPort.Shiftable.UniverseCellIndex);

	    var enable = false;
	    var thisEvent = sender.GetComponent<UniverseEvent>();
	    for (var i = -cellRadius; i < cellRadius + 1; i++)
	    {
	        for (var j = -cellRadius; j < cellRadius + 1; j++)
	        {
	            for (var k = -cellRadius; k < cellRadius + 1; k++)
	            {
	                if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex + new CellIndex(i, j, k)))
	                {
	                    enable = true;
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
        if (lastDistanceSquared > triggerRadiusSquared)
        {
            if (toViewPortSquared < triggerRadiusSquared)
            {
                Trigger();
            }
        }
        if (tracker == null)
        {
            if (lastDistanceSquared > trackerRadiusSquared)
            {
                if (toViewPortSquared < trackerRadiusSquared)
                {
                    tracker = gameObject.AddComponent<EventTracker>();
                    tracker.ArrowCursorImage = ArrowCursorImage;
                    tracker.TrackerCursorImage = TrackerCursorImage;
                    tracker.LabelFont = LabelFont;
                }
            }
        }
        if (tracker != null)
        {
            if (lastDistanceSquared < trackerRadiusSquared)
            {
                if (toViewPortSquared > trackerRadiusSquared)
                {
                    tracker.SelfDestroy();
                }
            }
        }
        lastDistanceSquared = toViewPortSquared;
    }

    private void OnDisable()
    {
        if (tracker != null)
            tracker.SelfDestroy();
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
        if (tracker != null)
            tracker.SelfDestroy();
    }
}
