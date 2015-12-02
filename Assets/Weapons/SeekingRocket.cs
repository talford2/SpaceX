using UnityEngine;

[RequireComponent(typeof(Shiftable))]
public class SeekingRocket : MonoBehaviour
{
	public float MissileSpeed;

	private bool _isLive;
	private Shiftable _shiftable;

	private GameObject _owner;
	private Vector3 _shootFrom;
	private Vector3 _initVelocity;
	private float _initSpeed;

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_shiftable.OnShift += Shift;
	}

	public void SetOwner(GameObject owner)
	{
		_owner = owner;
	}

	private void Update()
	{
		if (_isLive)
		{
			var displacement = (_initSpeed + MissileSpeed) * Time.deltaTime;
			_shiftable.Translate(transform.forward * displacement);
		}
	}

	public void Stop()
	{
		_isLive = false;
	}

	public void Shoot(Vector3 shootFrom, Vector3 direction, Vector3 initVelocity)
	{
		_isLive = true;
		_shootFrom = shootFrom;
		_initVelocity = initVelocity;
		_initSpeed = _initVelocity.magnitude;
		transform.position += initVelocity * Time.deltaTime;
		transform.forward = direction;
	}

	private void Shift(Shiftable sender, Vector3 delta)
	{
	}
}
