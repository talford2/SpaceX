using UnityEngine;

public class UniverseGenerator : MonoBehaviour
{
	public GameObject AsteroidPrefab;

	public int ObjectCount = 200;

	public float UniverseSize = 1000f;
	
	void Start()
	{
		for (var i = 0; i < ObjectCount; i++)
		{
			var inst = Instantiate(AsteroidPrefab);
			inst.transform.position = GetRandomPosition();
			inst.transform.rotation = Random.rotation;
		}
	}

	private Vector3 GetRandomPosition()
	{
		return Random.insideUnitSphere * UniverseSize;
	}
}
