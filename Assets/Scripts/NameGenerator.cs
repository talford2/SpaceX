using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NameGenerator
{
	private static List<string> _callSigns;

    private static List<string> _systemNames;

	private static List<string> CallSigns
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

    private static List<string> SystemNames
    {
        get
        {
            if (_systemNames == null)
            {
                var txt = Resources.Load<TextAsset>("SystemNames");
                _systemNames = txt.text.Split(',').ToList();
                Debug.Log(_systemNames);
            }
            return _systemNames;
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

    public static string GetRandomSystemName(int seed)
    {
        Random.InitState(seed);
        return GetRandomSystemName();
    }

    public static string GetRandomSystemName()
    {
        if (SystemNames.Any())
        {
            var index = Random.Range(0, SystemNames.Count);
            var name = SystemNames[index].Trim();
            SystemNames.RemoveAt(index);
            return name;
        }
        return "Unknown";
    }
}
