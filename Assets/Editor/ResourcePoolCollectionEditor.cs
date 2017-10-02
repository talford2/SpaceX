using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourcePoolCollection))]
public class ResourcePoolCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var resourcePoolCollection = (ResourcePoolCollection)target;

        //base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Prefab");
        GUILayout.Label("Pool Size");
        EditorGUILayout.EndHorizontal();

        var resources = resourcePoolCollection.Resources;

        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        if (resources.Any())
        {
            foreach (var resource in resources)
            {
                EditorGUILayout.BeginHorizontal();
                resource.Prefab = (GameObject)EditorGUILayout.ObjectField(resource.Prefab, typeof(GameObject), false);
                resource.PoolSize = EditorGUILayout.IntField(resource.PoolSize);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    resources.Remove(resource);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        if (GUILayout.Button("Add"))
        {
            resources.Add(null);
        }
        GUILayout.EndVertical();

        /*
        EditorGUILayout.Space();
        if (GUILayout.Button("From Children"))
        {
            //if ()
            resourcePoolCollection.Resources.Clear();
            foreach (Transform trans in resourcePoolCollection.transform)
            {
                if (trans != resourcePoolCollection.transform)
                {
                    var resourcePool = trans.GetComponent<ResourcePool>();
                    if (resourcePool != null)
                    {
                        resourcePoolCollection.Resources.Add(
                            new ResourcePoolCollection.ResourcePoolDefinition
                            {
                                Prefab = resourcePool.Prefab,
                                PoolSize = resourcePool.PoolSize
                            }
                        );
                    }
                }
            }
        }
        */
    }
}
