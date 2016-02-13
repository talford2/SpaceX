using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    public Collectible Item;
    public float Delay;

    private float _delayCooldown;
    private bool _isEnabled;

	private void Start ()
	{
	    _delayCooldown = Delay;
	    _isEnabled = false;
	}
	
	private void Update () {
	    if (_delayCooldown >= 0f)
	    {
	        _delayCooldown -= Time.deltaTime;
	        if (_delayCooldown < 0f)
	            _isEnabled = true;
	    }
	}

    public void Pickup(GameObject fromObject, Vector3 fromVelocity)
    {
        if (_isEnabled)
        {
            Item.SetVelocity(fromVelocity);
            Item.Collect(fromObject);
            _isEnabled = false;
        }
    }
}
