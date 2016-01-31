using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
	public static List<T> GameObjectList<T>(string label, List<T> objs, bool allowSceneObjects = true) where T : Object
	{
		if (objs == null)
		{
			objs = new List<T>();
		}
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		if (objs.Any())
		{
			for (var i = 0; i < objs.Count; i++)
			{
				GUILayout.BeginHorizontal();
				objs[i] = ObjectField<T>(objs[i], allowSceneObjects);
				if (GUILayout.Button("Remove", GUILayout.Width(60)))
				{
					objs.RemoveAt(i);
				}
				GUILayout.EndHorizontal();
			}
		}

		if (GUILayout.Button("Add"))
		{
			objs.Add(null);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		return objs;
	}

	public static T ObjectField<T>(T obj, bool allowSceneObjects) where T : Object
	{
		return (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects);
	}

	public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : Object
	{
		return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
	}

	public static EditorRange<float> FloatRange(string label, float min, float max)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		var range = new EditorRange<float>();
		range.Min = EditorGUILayout.FloatField(min);
		range.Max = EditorGUILayout.FloatField(max);
		EditorGUILayout.EndHorizontal();
		return range;
	}

	public static EditorRange<int> IntRange(string label, int min, int max)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		var range = new EditorRange<int>();
		range.Min = EditorGUILayout.IntField(min);
		range.Max = EditorGUILayout.IntField(max);
		EditorGUILayout.EndHorizontal();
		return range;
	}

	public static EditorRange<Color> ColorRange(string label, Color min, Color max)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		var range = new EditorRange<Color>();
		range.Min = EditorGUILayout.ColorField(min);
		range.Max = EditorGUILayout.ColorField(max);
		EditorGUILayout.EndHorizontal();
		return range;
	}

	public static int IntDropdown(string label, List<int> valList, int value)
	{
		return valList[EditorGUILayout.Popup(label, valList.IndexOf(value), valList.Select(v => v.ToString()).ToArray())];
	}
}

[System.Serializable]
public class EditorRange<T>
{
	public T Min;

	public T Max;
}