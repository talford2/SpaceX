using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventGenerator : MonoBehaviour
{
	public List<UniverseEventCount> UniverseEvents;

	public int CellRadius = 10;

	void Start()
	{
		Generate();
	}

	public void Generate()
	{
		var go = new GameObject();
		go.name = "UniverseEvents";
		var parent = go.transform;

		foreach (var ue in UniverseEvents)
		{
			for (var i = 0; i < ue.Count; i++)
			{
				var eventObj = Instantiate(ue.Prefab).GetComponent<UniverseEvent>();
				var shifter = eventObj.Shiftable;
				eventObj.transform.SetParent(parent);
				eventObj.transform.rotation = Random.rotation;
				shifter.UniverseCellIndex = new CellIndex(Random.insideUnitSphere * CellRadius);
				shifter.CellLocalPosition = Utility.RandomInsideCube * Universe.Current.CellSize;
				Universe.Current.UniverseEvents.Add(eventObj);
			}
		}
	}
}

[System.Serializable]
public class UniverseEventCount
{
	public GameObject Prefab;

	public int Count;
}