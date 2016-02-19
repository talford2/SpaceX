using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public Light DirectionLight;

	public List<GameObject> Levels;

	private List<UniverseGenerator> _uGenLevels;

	private List<Material> LevelBackgrounds;

	private ReflectionProbe _reflectionProbe;

	private int _levelIndex = 0;

	void Start()
	{
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
		_levelIndex++;
		if (_levelIndex < Levels.Count)
		{
			var uGen = Levels[_levelIndex].GetComponent<UniverseGenerator>();
			uGen.FinishedRendering += UGen_FinishedRendering;
			LevelBackgrounds.Add(uGen.GetMaterial());
		}
		ChangeLevel(0);
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.N))
		{
			_levelIndex++;
			if (_levelIndex > Levels.Count - 1)
			{
				_levelIndex = 0;
			}
			ChangeLevel(_levelIndex);
		}
	}

	private void ChangeLevel(int index)
	{
		RenderSettings.skybox = _uGenLevels[index].Background;
		DirectionLight.transform.forward = _uGenLevels[index].SunDirection;
		_reflectionProbe.RenderProbe();
	}
}
