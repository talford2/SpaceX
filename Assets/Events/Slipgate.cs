using UnityEngine;

public class Slipgate : MonoBehaviour
{
	public int LevelIndex = 0;

	private bool _isTriggered = false;

    void Update()
    {
        if (!_isTriggered && PlayerController.Current.VehicleInstance != null && (PlayerController.Current.VehicleInstance.Shiftable.GetWorldPosition() - transform.position).sqrMagnitude < 9)
        {
            Debug.Log("WARP!");
            _isTriggered = true;
            LevelManager.Current.ChangeLevel(LevelIndex);
        }
    }
}
