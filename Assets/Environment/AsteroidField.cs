using UnityEngine;

public class AsteroidField : MonoBehaviour
{
	public int AsteroidPoolCount = 1000;

	public Transform CentreTransform;

	public float FieldRadius = 300f;

	public GameObject AsteroidPrefab;

	private void Awake()
	{
		for (var i = 0; i < AsteroidPoolCount; i++)
		{
			var inst = Instantiate(AsteroidPrefab);
			inst.transform.position = Random.insideUnitSphere * FieldRadius + CentreTransform.position;
			inst.transform.rotation = Random.rotation;
			inst.transform.localScale = Vector3.one * Random.Range(0.3f, 2.3f);

			inst.transform.parent = transform;

			var levelOfDetail = inst.GetComponent<LevelOfDetail>();
			levelOfDetail.OnExitLargestDistance += MakeAvailable;
			levelOfDetail.OnEnterLargestDistance += MakeUnavailable;
		}
	}

	private void MakeAvailable(GameObject instance)
	{
		instance.transform.position = Random.onUnitSphere * FieldRadius + CentreTransform.position;
	}

	private void MakeUnavailable(GameObject instance)
	{
	}
}
