using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    public Collectible Item;
    public float Radius;
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
	    if (isEnabled)
	    {
            var hitColliders = Physics.OverlapSphere(transform.position, Radius, LayerMask.GetMask("Detectable"));
            foreach (var hitCollider in hitColliders)
            {
                var vehicle = hitCollider.GetComponent<Detectable>().TargetTransform;
                if (vehicle != null)
                {
                    if (PlayerController.Current.VehicleInstance != null)
                    {
                        if (vehicle.transform == PlayerController.Current.VehicleInstance.transform)
                        {
                            Item.SetVelocity(PlayerController.Current.VehicleInstance.GetVelocity());
                            Item.Collect(vehicle.gameObject);
                            isEnabled = false;
                        }
                    }
                }
            }
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
