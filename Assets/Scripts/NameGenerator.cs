using System.Collections.Generic;
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
				var txt = Resources.Load<TextAsset>("CallSigns");
				_callSigns = txt.text.Split(',').ToList();
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
