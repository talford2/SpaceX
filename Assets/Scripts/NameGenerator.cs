using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class NameGenerator
{
	private static List<string> _callSigns;

	public static List<string> CallSigns
	{
		get
		{
			if (_callSigns == null)
			{
				var filename = Path.Combine(Application.dataPath, "CallSigns.txt");
				var reader = new StreamReader(filename);
				var res = reader.ReadToEnd();
				reader.Close();
				_callSigns = res.Split(',').ToList();
				Debug.Log(_callSigns);
			}
			return _callSigns;
		}
	}

	public static string GetRandomCallSign()
	{
		if (CallSigns.Any())
		{
			var index = Random.Range(0, CallSigns.Count);
			var name = CallSigns[index].Trim();
			CallSigns.RemoveAt(index);
			return name;
		}
		return "Unknown";
	}
}
