using UnityEngine;

public class Slipgate : MonoBehaviour
{
	public int LevelIndex = 0;
	public float TriggerRadius = 20f;

	private bool _isTriggered = false;
	private float _lastDotProd;

    private void Start()
    {
        LevelIndex = LevelManager.Current.GetLevel().SlipGateLevelIndex;
    }

	private void Update()
	{
		if (!_isTriggered && Player.Current.VehicleInstance != null)
		{
			var dotProd = Vector3.Dot(Player.Current.VehicleInstance.Shiftable.GetWorldPosition() - transform.position, transform.forward);
			if ((Player.Current.VehicleInstance.Shiftable.GetWorldPosition() - transform.position).sqrMagnitude < TriggerRadius * TriggerRadius)
			{
				if (Mathf.Sign(dotProd) != Mathf.Sign(_lastDotProd))
				{
					Debug.LogFormat("WARP TO {0}!", LevelIndex);
					_isTriggered = true;
					LevelManager.Current.ChangeLevel(LevelIndex);
					LevelIndex += 1;
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
