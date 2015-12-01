using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ObjectList))]
public class ObjectListDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		EditorGUI.PropertyField(position, property.FindPropertyRelative("Objects"));
		EditorGUI.EndProperty();
	}
}

[System.Serializable]
public class ObjectList
{
	public List<Object> Objects;
}