using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Shiftable))]
public class UniverseEvent : MonoBehaviour
{
	public static List<UniverseEvent> UniverseEvents { get; set; }

	public Shiftable Shiftable { get; set; }

	public virtual void Awake()
	{
		if (UniverseEvents == null)
		{
			UniverseEvents = new List<UniverseEvent>();
		}

		Shiftable = GetComponent<Shiftable>();
		Shiftable.OnShift += Shiftable_OnShift;

		UniverseEvents.Add(this);
	}

	private void Shiftable_OnShift(Shiftable sender, Vector3 delta)
	{
		//Debug.Log(sender.UniverseCellIndex + " ==? " + Universe.Current.ViewPort.Shiftable.UniverseCellIndex);
		if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex))
		{
			//Debug.Log("Go!");

			Initialise();
		}
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
		Debug.Log("Event shift");
	}
}
