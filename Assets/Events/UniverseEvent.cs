using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Shiftable))]
public class UniverseEvent : MonoBehaviour
{
	public static List<UniverseEvent> UniverseEvents { get; set; }

	public Shiftable Shiftable { get; set; }

	private void Awake()
	{
		if (UniverseEvents == null)
		{
			UniverseEvents = new List<UniverseEvent>();
		}

		Shiftable = GetComponent<Shiftable>();
		Shiftable.OnCellIndexChange += Shiftable_OnCellIndexChange;

		UniverseEvents.Add(this);
	}

	private void Shiftable_OnCellIndexChange(Shiftable sender, CellIndex delta)
	{
		Debug.Log(sender.UniverseCellIndex + " ==? " + Universe.Current.ViewPort.Shiftable.UniverseCellIndex);
		if (sender.UniverseCellIndex.IsEqualTo(Universe.Current.ViewPort.Shiftable.UniverseCellIndex))
		{
			Initialise();
		}
	}

	private void OnDrawGizmos()
	{
		var children = GetComponentsInChildren<Transform>();
		Gizmos.color = Color.red;
		foreach (var child in children)
		{
			Gizmos.DrawLine(transform.position, child.position);
		}
		//gameObject.SetActive(false);
	}

	public virtual void Initialise()
	{
		Debug.Log("Event shift");
	}
}
