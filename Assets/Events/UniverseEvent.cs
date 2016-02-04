using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Shiftable))]
public class UniverseEvent : MonoBehaviour
{
	public static List<UniverseEvent> UniverseEvents { get; set; }

	public Shiftable Shiftable { get; set; }

    public float TriggerRadius;

    private float triggerRadiusSquared;
    private float lastDistanceSquared;

	public virtual void Awake()
	{
		if (UniverseEvents == null)
		{
			UniverseEvents = new List<UniverseEvent>();
		}

		Shiftable = GetComponent<Shiftable>();
		Shiftable.OnShift += Shiftable_OnShift;

	    triggerRadiusSquared = TriggerRadius*TriggerRadius;

		UniverseEvents.Add(this);
	}

	private void Shiftable_OnShift(Shiftable sender, Vector3 delta)
	{
		//Debug.Log(sender.UniverseCellIndex + " ==? " + Universe.Current.ViewPort.Shiftable.UniverseCellIndex);
	    sender.enabled = false;
	    for (var i = -1; i < 2; i++)
	    {
	        for (var j = -1; j < 2; j++)
	        {
	            for (var k = -1; k < 2; k++)
	            {
	                if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex + new CellIndex(i, j, k)))
	                {
	                    sender.enabled = true;
	                }
	            }
	        }
	    }

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
                Initialise();
            }
        }
        lastDistanceSquared = toViewPortSquared;
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

	public virtual void Initialise()
	{
		Debug.Log("Event triggered");
	}
}
