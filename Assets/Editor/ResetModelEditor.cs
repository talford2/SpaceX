using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ResetModel : EditorWindow
{
    public GameObject prefabModel = null;

    [MenuItem("Space X/Reset To Prefab")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ResetModel));
    }

    void OnGUI()
    {
        prefabModel = (GameObject)EditorGUILayout.ObjectField(prefabModel, typeof(GameObject), false);
        var selectObject = Selection.activeGameObject;
        if (prefabModel && selectObject)
        {
            if (GUILayout.Button("Reset"))
            {
                var selectDictionary = new Dictionary<string, Transform>();
                Debug.Log("Reset : " + prefabModel.name + " => " + selectObject.name);
                foreach (var selectPart in selectObject.GetComponentsInChildren<Transform>())
                {
                    var key = GenerateKey(selectPart, selectObject.transform);
                    if (selectDictionary.ContainsKey(key))
                    {
                        Debug.LogWarning("Key already exists: " + key);
                    }
                    else
                    {
                        selectDictionary.Add(key, selectPart);
                    }
                }

                foreach (var prefabPart in prefabModel.GetComponentsInChildren<Transform>())
                {
                    var key = GenerateKey(prefabPart, prefabModel.transform);
                    selectDictionary[key].localPosition = prefabPart.localPosition;
                    selectDictionary[key].localRotation = prefabPart.localRotation;
                    selectDictionary[key].localScale = prefabPart.localScale;
                }
            }
        }
    }

    private string GenerateKey(Transform trans, Transform top)
    {
        var key = "";
        GetAncestersAndSelf(trans, top).ForEach(a => key += "/" + a.name);
        return key;
    }

    private List<Transform> GetAncestersAndSelf(Transform obj, Transform top)
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
