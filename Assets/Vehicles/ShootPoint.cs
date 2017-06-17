using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    private GameObject _muzzlePrefab;

    public void Initialize(MuzzleFlash muzzlePrefab)
    {
        _muzzlePrefab = muzzlePrefab.gameObject;
    }

    public void Flash()
    {
        var _muzzleInstance = ResourcePoolManager.GetAvailable(_muzzlePrefab, transform).GetComponent<MuzzleFlash>();
        _muzzleInstance.Flash();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
