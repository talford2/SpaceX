using UnityEngine;

public class Utility {
    public static Transform FindOrCreateContainer(string name)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
        {
            if (existing.transform.parent == null)
            {
                return existing.transform;
            }
        }
        return new GameObject(name).transform;
    }

    public static GameObject InstantiateInParent(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, int layer)
    {
        var instance = (GameObject)Object.Instantiate(gameObject, position, rotation);
        instance.transform.SetParent(parent);
        instance.layer = layer;
        return instance;
    }

    public static GameObject InstantiateInParent(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
    {
        return InstantiateInParent(gameObject, position, rotation, parent, parent.gameObject.layer);
    }

    public static GameObject InstantiateInParent(GameObject gameObject, Transform parent)
    {
        return InstantiateInParent(gameObject, parent.position, parent.rotation, parent, parent.gameObject.layer);
    }
}
