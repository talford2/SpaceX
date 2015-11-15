using UnityEngine;

public class AsteroidField : MonoBehaviour
{
	public int AsteroidPoolCount = 1000;

	public Transform CentreTransform;

	public float FieldRadius = 300f;

    public float FieldOfView = 60f;

	public GameObject AsteroidPrefab;

    private float _halfFov;

	private void Awake()
	{
	    _halfFov = FieldOfView/2f;
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
        var randomInFront = Quaternion.Euler(Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov)) * Universe.Current.ViewPort.transform.forward;
        instance.transform.position = randomInFront * FieldRadius + CentreTransform.position;
	}

	private void MakeUnavailable(GameObject instance)
	{
	}
}
