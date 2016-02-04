using Effects;
using UnityEngine;

public class Collectible : MonoBehaviour
{
	public AudioClip SoundClip;
	public float RotateSpeed;

	private bool isCollected;
	private Transform collectorTransform;
	private Shiftable shiftable;
	private Vector3 velocity;
	private Vector3 rotateSpeed;

    private SelfDestructor destructor;

	private float followAcceleration = 250f;
	private float followSpeed;
	private Vector3 lastTo;
	private float inheritVelocityFraction;
	private float inheritVelocityFractionSpeed = 0.4f;

	public Shiftable Shiftable { get { return shiftable; } }

	private void Awake()
	{
		shiftable = GetComponent<Shiftable>();
		rotateSpeed = Random.insideUnitSphere * RotateSpeed;
	    destructor = GetComponent<SelfDestructor>();
	}

	public void Collect(GameObject collector)
	{
		if (collector.transform == PlayerController.Current.VehicleInstance.transform)
		{
			isCollected = true;
			collectorTransform = collector.transform;
			inheritVelocityFraction = 0.6f;
		}
	}

	private bool isFinished = false;

	private void Update()
	{
	    if (destructor != null)
	    {
	        if ((transform.position - Universe.Current.ViewPort.Shiftable.GetWorldPosition()).sqrMagnitude < 1000f*1000f)
	        {
	            destructor.Cooldown = 15f;
	        }
	    }
	    if (!isFinished && isCollected && collectorTransform != null)
		{
			if (PlayerController.Current.VehicleInstance != null)
			{
				if (inheritVelocityFraction < 1f)
				{
					inheritVelocityFractionSpeed += inheritVelocityFractionSpeed * Time.deltaTime;
					if (inheritVelocityFraction > 1f)
						inheritVelocityFraction = 1f;
				}
				velocity = inheritVelocityFraction * PlayerController.Current.VehicleInstance.GetVelocity();
			}

			var toCollector = transform.position - collectorTransform.position;
			followSpeed += followAcceleration * Time.deltaTime;
			velocity -= followSpeed * toCollector.normalized;

			toCollector = transform.position - collectorTransform.position;
			var dotProd = Vector3.Dot(lastTo, toCollector);
			if (dotProd < 0f)
			{
				isFinished = true;
				Debug.Log("COLLECTED!");
				if (SoundClip != null)
				{
					Utility.PlayOnTransform(SoundClip, PlayerController.Current.VehicleInstance.transform);
				}
				if (PlayerController.Current != null)
				{
					PlayerController.Current.SpaceJunkCount++;
				}
				Destroy(gameObject);
			}
			lastTo = toCollector;
		}
		transform.rotation *= Quaternion.Euler(rotateSpeed * Time.deltaTime);
		var displacement = velocity * Time.deltaTime;
		shiftable.Translate(displacement);
	}

	public void SetVelocity(Vector3 value)
	{
		velocity = value;
	}
}
