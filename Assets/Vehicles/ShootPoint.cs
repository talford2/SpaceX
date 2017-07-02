using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    public void Initialize() { }

    public void Flash(MuzzleFlash _muzzlePrefab)
    {
        var _muzzleInstance = ResourcePoolManager.GetAvailable(_muzzlePrefab.gameObject, transform).GetComponent<MuzzleFlash>();
        _muzzleInstance.Flash();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
    }
}
