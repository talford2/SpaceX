using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    private MuzzleFlash _muzzleInstance;

    public void Initialize(MuzzleFlash muzzlePrefab)
    {
        _muzzleInstance = Utility.InstantiateInParent(muzzlePrefab.gameObject, transform).GetComponent<MuzzleFlash>();
    }

    public void Flash()
    {
        _muzzleInstance.Flash();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
