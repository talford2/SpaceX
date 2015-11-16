using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    public MuzzleFlash MuzzlePrefab;

    private MuzzleFlash _muzzleInstance;

    public void Initialize()
    {
        _muzzleInstance = Utility.InstantiateInParent(MuzzlePrefab.gameObject, transform).GetComponent<MuzzleFlash>();
    }

    public void Flash()
    {
        _muzzleInstance.Flash();
    }
}
