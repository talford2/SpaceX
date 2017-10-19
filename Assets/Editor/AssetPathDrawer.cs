using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AssetPathAttribute))]
public class AssetPathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var filter = "*.asset";
        var prefabPath = attribute as AssetPathAttribute;
        var path = prefabPath.Path;

        var prefabNames = EditorExtensions.PrefabNamesFromPath(path, filter);
        var options = new List<GUIContent>();
        foreach (var prefabName in prefabNames)
        {
            options.Add(new GUIContent(string.Format("{0} ({1})", prefabName, property.objectReferenceValue.GetType())));
        }

        EditorGUI.BeginProperty(position, label, property);
        var index = EditorExtensions.GetAssetPathIndexFor(path, property.objectReferenceValue.name);
        index = EditorGUI.Popup(position, label, index, options.ToArray());
        property.objectReferenceValue = EditorExtensions.FromPathDropdownIndex<Object>(path, index, filter);
        EditorGUI.EndProperty();
    }
}
