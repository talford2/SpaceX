using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabPathAttribute))]
public class PrefabPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var filter = "*.prefab";
        var prefabPath = attribute as PrefabPathAttribute;
        var path = prefabPath.Path;

        var prefabNames = EditorExtensions.PrefabNamesFromPath(path, filter);
        var options = new List<GUIContent>();
        foreach (var prefabName in prefabNames)
        {
            options.Add(new GUIContent(string.Format("{0} ({1})", prefabName, property.objectReferenceValue.GetType())));
        }

        EditorGUI.BeginProperty(position, label, property);
        var index = EditorExtensions.GetPrefabPathIndexFor(path, property.objectReferenceValue.name);
        index = EditorGUI.Popup(position, label, index, options.ToArray());
        property.objectReferenceValue = EditorExtensions.FromPathDropdownIndex<Object>(path, index, filter);
        EditorGUI.EndProperty();
    }
}
