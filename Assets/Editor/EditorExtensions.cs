using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
	public static List<GameObject> GameObjectList(string label, List<GameObject> objs)
	{
		if (objs == null)
		{
			objs = new List<GameObject>();
		}
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		if (objs.Any())
		{
			for (var i = 0; i < objs.Count; i++)
			{
				GUILayout.BeginHorizontal();
				objs[i] = ObjectField<GameObject>(objs[i], true);
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
}