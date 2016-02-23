using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Dialogue
{
	private static Dictionary<string, List<string>> _dialogueDictionary;

	public static Dictionary<string, List<string>> DialogueDictionary
	{
		get
		{
			if (_dialogueDictionary == null)
			{
				_dialogueDictionary = Parse(Resources.Load<TextAsset>("Dialogue").text);
			}
			return _dialogueDictionary;
		}
	}

	public static string GetRandomDialogue(string key, params object[] args)
	{
		return string.Format(DialogueDictionary[key][UnityEngine.Random.Range(0, DialogueDictionary[key].Count)], args);
	}

	private static Dictionary<string, List<string>> Parse(string text)
	{
		var dictionary = new Dictionary<string, List<string>>();
		var strRdr = new StringReader(text);
		var line = "";
		var key = "";
		while ((line = strRdr.ReadLine()) != null)
		{
			if (!line.StartsWith("\""))
			{
				key = line.Trim().TrimEnd(':');
				dictionary.Add(key, new List<string>());
			}
			else
			{
				dictionary[key].Add(line.TrimStart('"').TrimEnd('"'));
			}
		}
		return dictionary;
	}
}