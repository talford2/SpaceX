using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour
{
    public List<GameObject> StarPrefabs;
    public float MinSize;
    public float MaxSize;
    public float Radius;
    public int Count;

    private void Awake()
    {
        for (var i = 0; i < Count; i++)
        {
            var position = Radius*Random.onUnitSphere;
            var star = Utility.InstantiateInParent(StarPrefabs[Random.Range(0, StarPrefabs.Count)], position, Quaternion.identity, transform);
            Utility.SetLayerRecursively(star, LayerMask.NameToLayer("Universe Background"));
            star.transform.localScale = Vector3.one*Random.Range(MinSize, MaxSize);
            star.transform.LookAt(Camera.main.transform, transform.up);
        }
    }
}
