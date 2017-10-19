using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static void MatchToMesh(GameObject instance, GameObject sourceMesh)
    {
        var instanceDictionary = new Dictionary<string, Transform>();
        foreach (var instancePart in instance.GetComponentsInChildren<Transform>())
        {
            var key = GenerateKey(instancePart, instance.transform);
            if (instanceDictionary.ContainsKey(key))
            {
                Debug.LogWarningFormat("Instance key already exists: '{0}'", key);
            }
            else
            {
                instanceDictionary.Add(key, instancePart);
            }
        }
        foreach (var sourcePart in sourceMesh.GetComponentsInChildren<Transform>())
        {
            var key = GenerateKey(sourcePart, sourceMesh.transform);
            if (!instanceDictionary.ContainsKey(key))
            {
                Debug.LogWarningFormat("Source mesh does not contain transform matching key: '{0}'", key);
                var newMesh = Object.Instantiate(sourcePart.gameObject, instance.transform);
            }
            else
            {
                MatchLocalTransforms(instanceDictionary[key], sourcePart);
            }
        }
    }

    public static void MatchLocalTransforms(Transform modifyTransform, Transform sourceTransform)
    {
        modifyTransform.localPosition = sourceTransform.localPosition;
        modifyTransform.localRotation = sourceTransform.localRotation;
        modifyTransform.localScale = sourceTransform.localScale;
    }

    public static void ResetLocalTransform(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private static string GenerateKey(Transform trans, Transform top)
    {
        var key = string.Empty;
        GetAncestersAndSelf(trans, top).ForEach(a => key += "/" + a.name);
        return key;
    }

    private static List<Transform> GetAncestersAndSelf(Transform obj, Transform top)
    {
        var list = new List<Transform>();
        var curParent = obj.transform;
        while (curParent != null && curParent.name != top.name)
        {
            list.Add(curParent);
            curParent = curParent.parent;
        }
        list.Add(top);
        list.Reverse();
        return list;
    }
}
