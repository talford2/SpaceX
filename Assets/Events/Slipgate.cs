using UnityEngine;

public class Slipgate : MonoBehaviour
{
	public int LevelIndex = 0;
    public float TriggerRadius = 20f;

	private bool _isTriggered = false;
    private float _lastDotProd;

    void Update()
    {
        if (!_isTriggered && PlayerController.Current.VehicleInstance != null)
        {
            var dotProd = Vector3.Dot(PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition() - transform.position, transform.forward);
            if ((PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition() - transform.position).sqrMagnitude < TriggerRadius * TriggerRadius)
            {
                if (Mathf.Sign(dotProd) != Mathf.Sign(_lastDotProd))
                {
                    Debug.Log("WARP!");
                    _isTriggered = true;
                    LevelManager.Current.ChangeLevel(LevelIndex);
                }
            }
            _lastDotProd = dotProd;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, TriggerRadius);
    }
}
