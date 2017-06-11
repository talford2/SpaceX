using UnityEngine;

public class ResourcePoolIndex : MonoBehaviour
{
    public GameObject AnonymousSoundPrefab;

    private static ResourcePoolIndex _current;

    public static GameObject AnonymousSound { get { return _current.AnonymousSoundPrefab; } }

    private void Awake()
    {
        _current = this;
    }
}
