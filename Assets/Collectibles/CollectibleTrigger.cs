using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    public Collectible Item;
    public float Delay;

    private float delayCooldown;
    private bool isEnabled;

	private void Start ()
	{
	    delayCooldown = Delay;
	    isEnabled = false;
	}
	
	private void Update () {
	    if (delayCooldown >= 0f)
	    {
	        delayCooldown -= Time.deltaTime;
	        if (delayCooldown < 0f)
	            isEnabled = true;
	    }
	}

    public void Pickup(GameObject fromObject, Vector3 fromVelocity)
    {
        if (isEnabled)
        {
            Item.SetVelocity(fromVelocity);
            Item.Collect(fromObject);
            isEnabled = false;
        }
    }
}
