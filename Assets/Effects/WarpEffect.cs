using UnityEngine;

public class WarpEffect : MonoBehaviour
{
	private float _naturalScale = 1f;

	public float Timeout = 1f;

	public float Distance = 20f;

	public GameObject WarpEffectPrefab;

	private float _cooldown;
	private UniversePosition _finalDestination;
    
	private Vector3 _warpPos;
    private Shiftable _shiftable;

	void Awake()
	{
		_naturalScale = transform.localScale.x;
		_finalDestination = Universe.Current.GetUniversePosition(transform.position);

		_warpPos = transform.position - (transform.forward * Distance);

		_cooldown = Timeout;

	    _shiftable = GetComponent<Shiftable>();

	    //_rigidBody = GetComponent<Rigidbody>();
	    //_rigidBody.isKinematic = true;
	}

	void Start()
	{
		var effect = Instantiate<GameObject>(WarpEffectPrefab);
		effect.transform.position = _warpPos;
	}

	void Update()
	{
		// 0 to 1
		var frac = 1 - _cooldown / Timeout;

	    var pos = Vector3.Lerp(_shiftable.GetWorldPosition(), Universe.Current.GetWorldPosition(_finalDestination), frac);
	    var displacement = pos - _shiftable.GetWorldPosition();
        _shiftable.Translate(displacement);

        //transform.position = Vector3.Lerp(_warpPos, _finalDestination, frac);
        //transform.localScale = _naturalScale * Vector3.one * frac;
        _cooldown -= Time.deltaTime;
		if (_cooldown < 0)
		{
			//_rigidBody.isKinematic = false;
			Destroy(this);
		}
	}
}
