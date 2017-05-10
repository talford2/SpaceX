using UnityEngine;
using System.Collections.Generic;

public class EventGenerator : MonoBehaviour
{
	//public List<UniverseEventCount> UniverseEvents;

	public int CellRadius = 10;

	public void Generate(List<UniverseEventCount> universeEvents, int seed)
	{
        Random.InitState(seed);
		var go = new GameObject("UniverseEvents");
		var parent = go.transform;
		foreach (var ue in universeEvents)
		{
			for (var i = 0; i < ue.Count; i++)
			{
				var eventObj = Instantiate(ue.Prefab).GetComponent<UniverseEvent>();
				var shifter = eventObj.Shiftable;
				eventObj.transform.SetParent(parent);
				eventObj.transform.rotation = Random.rotation;
				shifter.UniverseCellIndex = new CellIndex(Random.insideUnitSphere * CellRadius);
			    shifter.CellLocalPosition = Utility.RandomInsideCube*Universe.Current.CellSize - Universe.Current.CellSize*0.5f*Vector3.one;
				Universe.Current.UniverseEvents.Add(eventObj);
			}
		}
	}

    public void Clear()
    {
        for (var i = 0; i < Universe.Current.UniverseEvents.Count; i++)
        {
            Destroy(Universe.Current.UniverseEvents[i].gameObject);
        }
        Universe.Current.UniverseEvents.Clear();
    }

    /*
    public void Generate(int seed)
    {
        Random.seed = seed;
        Debug.Log("EVENTS FROM SEED: " + Random.seed);
        Generate();
    }
    */
}

[System.Serializable]
public class UniverseEventCount
{
	public GameObject Prefab;

	public int Count;
}