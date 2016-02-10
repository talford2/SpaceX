using Effects;
using UnityEngine;

public class Collectible : MonoBehaviour
{
	public AudioClip SoundClip;
	public float RotateSpeed;

	public float Lifetime = 15;
	public GameObject FadeObject;
	public float FadeTime = 1;

	private bool _isCollected;
	private Transform _collectorTransform;
	private Shiftable _shiftable;
	private Vector3 _velocity;
	private Vector3 _rotateSpeed;

	private SelfDestructor _destructor;

	private float _followAcceleration = 250f;
	private float _followSpeed;
	private Vector3 _lastTo;
	private float _inheritVelocityFraction;
	private float _inheritVelocityFractionSpeed = 0.4f;

	private float _lifeTimeCooldown = 0;
	private float _fadeCooldown = 0;

	public Shiftable Shiftable { get { return _shiftable; } }
	
	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_rotateSpeed = Random.insideUnitSphere * RotateSpeed;
		_destructor = GetComponent<SelfDestructor>();
		_lifeTimeCooldown = Lifetime;
		_fadeCooldown = FadeTime;
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
					PlayerController.Current.SpaceJunkCount++;
				}
				Destroy(gameObject);
			}
			_lastTo = toCollector;
		}
		transform.rotation *= Quaternion.Euler(_rotateSpeed * Time.deltaTime);
		var displacement = _velocity * Time.deltaTime;
		_shiftable.Translate(displacement);

		if ((_lifeTimeCooldown - FadeTime) < 0)
		{
			_fadeCooldown -= Time.deltaTime;
			var frac = _fadeCooldown / FadeTime;
			FadeObject.transform.localScale = Vector3.one * frac;
			if (frac < 0)
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
