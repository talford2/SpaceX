using UnityEngine;

public class MothershipBay : MonoBehaviour
{
    public ProximitySpawner AttachedSpanwer;

    private void OnDestroy()
    {
        Destroy(AttachedSpanwer.gameObject);
    }
}
