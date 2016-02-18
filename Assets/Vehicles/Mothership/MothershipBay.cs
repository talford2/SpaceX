using UnityEngine;

public class MothershipBay : MonoBehaviour
{
    public ProximitySpawner AttachedSpawner;

    private void OnDestroy()
    {
        //Debug.Log("DESTROY: " + AttachedSpawner.name + "!");
        Destroy(AttachedSpawner.gameObject);
    }
}
