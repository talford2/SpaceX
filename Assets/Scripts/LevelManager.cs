using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public Light DirectionLight;

	public List<GameObject> Levels;

	private List<UniverseGenerator> _uGenLevels;

	private List<Material> LevelBackgrounds;

	private ReflectionProbe _reflectionProbe;

	private int _buildIndex = 0;

	private int _viewIndex = 0;

	private static LevelManager _current;

	void Start()
	{
		_current = this;

		LevelBackgrounds = new List<Material>();
		_uGenLevels = new List<UniverseGenerator>();
		foreach (var go in Levels)
		{
			_uGenLevels.Add(go.GetComponent<UniverseGenerator>());
		}

		var uGen = Levels[0].GetComponent<UniverseGenerator>();
		uGen.FinishedRendering += UGen_FinishedRendering;
		LevelBackgrounds.Add(uGen.GetMaterial());

		// Reflection probe
		var g = new GameObject();
		g.name = "System Reflection Probe";
		_reflectionProbe = g.AddComponent<ReflectionProbe>();
		_reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;// UnityEngine.Rendering.ReflectionProbeMode.Custom;
		_reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		_reflectionProbe.cullingMask = LayerMask.GetMask("Universe Backgrouund");
		_reflectionProbe.size = Vector3.one * (Universe.Current.CellSize + 100);
		_reflectionProbe.transform.position = Vector3.zero;
	}

	private void UGen_FinishedRendering()
	{
		_buildIndex++;
		if (_buildIndex < Levels.Count)
		{
			var uGen = Levels[_buildIndex].GetComponent<UniverseGenerator>();
			uGen.FinishedRendering += UGen_FinishedRendering;
			LevelBackgrounds.Add(uGen.GetMaterial());
		}

		if (_buildIndex == Levels.Count - 1)
		{
			ChangeLevel(_viewIndex);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.N))
		{
			_viewIndex++;
			if (_viewIndex > Levels.Count - 1)
			{
				_viewIndex = 0;
			}
			ChangeLevel(_viewIndex);
		}
	}

	public void ChangeLevel(int index)
	{
		RenderSettings.skybox = _uGenLevels[index].Background;
		DirectionLight.transform.forward = _uGenLevels[index].SunDirection;
		DirectionLight.color = _uGenLevels[index].SunColour;
		DirectionLight.intensity = _uGenLevels[index].SunIntensity;

		_reflectionProbe.RenderProbe();
	}

	public static LevelManager Current
	{
		get
		{
			return _current;
		}
	}
}
