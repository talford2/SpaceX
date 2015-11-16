using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    public MuzzleFlash MuzzlePrefab;

    private MuzzleFlash _muzzleInstance;
    private Vector3 _offset;

    public Vector3 Offset { get { return _offset; } }

    public void Initialize(Vector3 centre)
    {
        _muzzleInstance = Utility.InstantiateInParent(MuzzlePrefab.gameObject, transform).GetComponent<MuzzleFlash>();
        _offset = transform.position - centre;
    }

    public void Flash()
    {
        _muzzleInstance.Flash();
    }
}
