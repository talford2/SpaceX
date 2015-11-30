using UnityEngine;

public class Starfield : MonoBehaviour
{
    public GameObject StarPrefab;
    public float MinSize;
    public float MaxSize;
    public float Radius;
    public int Count;

    private void Awake()
    {
        for (var i = 0; i < Count; i++)
        {
            var position = Radius*Random.onUnitSphere;
            var star = Utility.InstantiateInParent(StarPrefab, position, Quaternion.identity, transform, LayerMask.NameToLayer("Universe Background"));
            star.transform.localScale = Vector3.one*Random.Range(MinSize, MaxSize);
            star.transform.LookAt(Camera.main.transform, transform.up);
        }
    }
}
