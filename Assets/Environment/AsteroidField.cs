using UnityEngine;

public class AsteroidField : MonoBehaviour
{
	public int AsteroidPoolCount = 1000;

	public Transform CentreTransform;

	public float FieldRadius = 300f;

	public float FieldOfView = 60f;

	public GameObject AsteroidPrefab;

	public float MinSize = 0.3f;

	public float MaxSize = 3.0f;

	private float _halfFov;

	private void Awake()
	{
		_halfFov = FieldOfView / 2f;
		for (var i = 0; i < AsteroidPoolCount; i++)
		{
			var inst = Instantiate(AsteroidPrefab);

			inst.transform.position = Random.insideUnitSphere * FieldRadius + CentreTransform.position;
			inst.transform.rotation = Random.rotation;
			inst.GetComponent<ScaleOnEnable>().ResetToZero(Random.Range(MinSize, MaxSize));

			inst.transform.parent = transform;

			inst.GetComponent<Killable>().OnDie += AsteroidField_OnDie;

			var levelOfDetail = inst.GetComponent<LevelOfDetail>();
			levelOfDetail.OnExitLargestDistance += MakeAvailable;
		}
	}

	private void AsteroidField_OnDie(Killable sender)
	{
		// TODO: This isn't perfect, whenever an asteroid is destroyed a new one will be respawned,
		// really the asteroid should just be hidden and reset when it falls out of the range
		MakeAvailable(sender.gameObject);
	}

	private void MakeAvailable(GameObject instance)
	{
		var randomInFront = Quaternion.Euler(Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov), Random.Range(-_halfFov, _halfFov)) * Universe.Current.ViewPort.transform.forward;
		instance.transform.position = randomInFront * FieldRadius + CentreTransform.position;
		instance.GetComponent<ScaleOnEnable>().ResetToZero(Random.Range(MinSize, MaxSize));
	}
}
