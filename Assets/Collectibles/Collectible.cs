using Effects;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectType GiveType;
	public AudioClip SoundClip;
	public float RotateSpeed;

	public float MinLifetime = 14;
	public float MaxLifetime = 15;

	public GameObject FadeObject;
	public float FadeTime = 1;

	public AnimationCurve FadeOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

	private bool _isCollected;
	private Transform _collectorTransform;
	private Shiftable _shiftable;
	private Vector3 _velocity;
	private Vector3 _rotateSpeed;

	private CollectibleTracker _collectibleTracker;
	private SelfDestructor _destructor;

	private float _followAcceleration = 250f;
	private float _followSpeed;
	private Vector3 _lastTo;
	private float _inheritVelocityFraction;
	private float _inheritVelocityFractionSpeed = 0.4f;

	private float _lifeTimeCooldown = 0;
	private float _fadeCooldown = 0;

    public Shiftable Shiftable;

	private void Awake()
	{
		_rotateSpeed = Random.insideUnitSphere * RotateSpeed;
		_destructor = GetComponent<SelfDestructor>();
		_lifeTimeCooldown = Random.Range(MinLifetime, MaxLifetime);

		_fadeCooldown = FadeTime;
		_collectibleTracker = GetComponent<CollectibleTracker>();
	}

	public void Collect(GameObject collector)
	{
		if (collector.transform == PlayerController.Current.VehicleInstance.transform)
		{
			_isCollected = true;
			_collectorTransform = collector.transform;
			_inheritVelocityFraction = 0.6f;
		}
	}

	private bool isFinished = false;

	private void Update()
	{
		if (_destructor != null)
		{
			if ((transform.position - Universe.Current.ViewPort.Shiftable.GetWorldPosition()).sqrMagnitude < 1000f * 1000f)
			{
				_destructor.Cooldown = 15f;
			}
		}
		if (!isFinished && _isCollected && _collectorTransform != null)
		{
			if (PlayerController.Current.VehicleInstance != null)
			{
				if (_inheritVelocityFraction < 1f)
				{
					_inheritVelocityFractionSpeed += _inheritVelocityFractionSpeed * Time.deltaTime;
					if (_inheritVelocityFraction > 1f)
						_inheritVelocityFraction = 1f;
				}
				_velocity = _inheritVelocityFraction * PlayerController.Current.VehicleInstance.GetVelocity();
			}

			var toCollector = transform.position - _collectorTransform.position;
			_followSpeed += _followAcceleration * Time.deltaTime;
			_velocity -= _followSpeed * toCollector.normalized;

			toCollector = transform.position - _collectorTransform.position;
			var dotProd = Vector3.Dot(_lastTo, toCollector);
			if (dotProd < 0f)
			{
				isFinished = true;
				if (SoundClip != null)
				{
					Utility.PlayOnTransform(SoundClip, PlayerController.Current.VehicleInstance.transform);
				}
				if (PlayerController.Current != null)
				{
				    if (GiveType == CollectType.SpaceJunk)
				        PlayerController.Current.SpaceJunkCount++;
				    if (GiveType == CollectType.PowerNode)
				        PlayerController.Current.PowerNodeCount++;
				}
				Destroy(gameObject);
			}
			_lastTo = toCollector;
		}
		transform.rotation *= Quaternion.Euler(_rotateSpeed * Time.deltaTime);
		var displacement = _velocity * Time.deltaTime;
		_shiftable.Translate(displacement);

		if ((_lifeTimeCooldown - FadeTime) < 0f)
		{
			var linearFrac = _fadeCooldown / FadeTime;

			var finalFrac = linearFrac;
			if (FadeOutCurve != null)
			{
				finalFrac = FadeOutCurve.Evaluate(linearFrac);
			}

			FadeObject.transform.localScale = Vector3.one * finalFrac;
			_fadeCooldown -= Time.deltaTime;

			_collectibleTracker.SetAlpha(finalFrac);
			_collectibleTracker.SetScale(finalFrac);

			if (linearFrac < 0f)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			_lifeTimeCooldown -= Time.deltaTime;
		}
	}

	public void SetVelocity(Vector3 value)
	{
		_velocity = value;
	}
}
