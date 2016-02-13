using System.Collections;
using UnityEngine;

public class DroneHive : MonoBehaviour
{
    public Spawner DroneSpawner;
    public Transform TopDoor;
    public Transform BottomDoor;
    public MeshRenderer DroneCopy;

    private bool _isReleasingDrones;

    public void ReleaseDrones(int amount)
    {
        if (!_isReleasingDrones)
        {
            DroneCopy.enabled = true;
            Debug.Log("RELEASE THE DRONES!");
            _isReleasingDrones = true;
            StartCoroutine(OpenAndCloseDoors());
        }
    }

    private IEnumerator OpenAndCloseDoors()
    {
        for (var fraction = 0f; fraction < 1f; fraction += 0.05f)
        {
            TopDoor.localRotation = Quaternion.Lerp(TopDoor.localRotation, Quaternion.Euler(35f, 0f, 0f), fraction);
            BottomDoor.localRotation = Quaternion.Lerp(BottomDoor.localRotation, Quaternion.Euler(-35f, 0f, 0f), fraction);
            yield return new WaitForSeconds(0.01f);
        }
        TopDoor.localRotation = Quaternion.Euler(35f, 0f, 0f);
        BottomDoor.localRotation = Quaternion.Euler(-35f, 0f, 0f);
        DroneSpawner.Spawn(0f, () =>
        {
            DroneCopy.enabled = false;
        });
        DroneSpawner.Reset();
        yield return new WaitForSeconds(0.5f);
        for (var fraction = 0f; fraction < 1f; fraction += 0.05f)
        {
            TopDoor.localRotation = Quaternion.Lerp(TopDoor.localRotation, Quaternion.Euler(0f, 0f, 0f), fraction);
            BottomDoor.localRotation = Quaternion.Lerp(BottomDoor.localRotation, Quaternion.Euler(0f, 0f, 0f), fraction);
            yield return new WaitForSeconds(0.01f);
        }
        TopDoor.localRotation = Quaternion.Euler(0f, 0f, 0f);
        BottomDoor.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _isReleasingDrones = false;
    }
}
