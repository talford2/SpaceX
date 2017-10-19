using System.Collections.Generic;
using System.IO;
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

    // Assets from path
    private static string[] GetPrefabFilenames(string path, string filter)
    {
        var assetParentPath = new DirectoryInfo(Application.dataPath).Parent;
        return Directory.GetFiles(string.Format("{0}/{1}", assetParentPath, path), filter);
    }

    private static List<string> PrefabNamesFromPath(string path, string filter)
    {
        var prefabFiles = GetPrefabFilenames(path, filter);
        var prefabNames = new List<string>();
        foreach (var file in prefabFiles)
        {
            var filename = new FileInfo(file).Name;
            var prefab = AssetDatabase.LoadAssetAtPath<Object>(string.Format("{0}/{1}", path, filename));
            prefabNames.Add(prefab.name);
        }
        return prefabNames;
    }

    private static int GetPathDropdownIndexFor(string path, string prefabName, string filter)
    {
        var prefabNames = PrefabNamesFromPath(path, filter).ToArray();
        for (var i = 0; i < prefabNames.Length; i++)
        {
            if (prefabName.Equals(prefabNames[i]))
                return i;
        }
        Debug.LogWarningFormat("Could not file prefab named '{0}' under '{1}' applying filter '{2}'.", prefabName, path, filter);
        return -1;
    }

    public static int GetPrefabPathIndexFor(string path, string prefabName)
    {
        return GetPathDropdownIndexFor(path, prefabName, "*.prefab");
    }

    public static int GetAssetPathIndexFor(string path, string assetName)
    {
        return GetPathDropdownIndexFor(path, assetName, "*.asset");
    }

    private static T FromPathDropdownIndex<T>(string path, int index, string filter) where T : Object
    {
        var prefabFiles = GetPrefabFilenames(path, filter);
        var prefabFilename = new FileInfo(prefabFiles[index]).Name;
        return AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}/{1}", path, prefabFilename));
    }

    private static int PathDropdown(string path, string label, int selectedIndex, string filter = "*.prefab")
    {
        var prefabNames = PrefabNamesFromPath(path, filter).ToArray();
        return EditorGUILayout.Popup(label, selectedIndex, prefabNames);
    }

    // Simple Fields
    public static T FieldFor<T>(string label, T value) where T : Object
    {
        return (T)EditorGUILayout.ObjectField(label, value, typeof(T), false);
    }

    public static string FieldFor(string label, string value)
    {
        return EditorGUILayout.TextField(label, value);
    }

    public static float FieldFor(string label, float value)
    {
        return EditorGUILayout.FloatField(label, value);
    }

    private static T DropdownPathFieldFor<T>(string label, string path, string filter, ref int index, T value) where T : Object
    {
        index = PathDropdown(path, label, index, filter);
        return FromPathDropdownIndex<T>(path, index, filter);
    }

    public static T PrefabPathFieldFor<T>(string label, string path, ref int index, T value) where T: Object
    {
        return DropdownPathFieldFor(label, path, "*.prefab", ref index, value);
    }

    public static T AssetPathFieldFor<T>(string label, string path, ref int index, T value) where T : Object
    {
        return DropdownPathFieldFor(label, path, "*.asset", ref index, value);
    }
}

[System.Serializable]
public class EditorRange<T>
{
	public T Min;

	public T Max;
}