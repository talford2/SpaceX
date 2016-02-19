using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public Light DirectionLight;

	public List<GameObject> Levels;

	private List<UniverseGenerator> _uGenLevels;

	private List<Material> LevelBackgrounds;

	private int _levelIndex = 0;

	void Start()
	{
		_uGenLevels = new List<UniverseGenerator>();
		foreach (var go in Levels)
		{
			_uGenLevels.Add(go.GetComponent<UniverseGenerator>());
		}
		LevelBackgrounds = new List<Material>();

		var uGen = Levels[0].GetComponent<UniverseGenerator>();
		uGen.FinishedRendering += UGen_FinishedRendering;
		LevelBackgrounds.Add(uGen.GetMaterial());

		//ChangeLevel(0);
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

		//foreach (var lvl in _uGenLevels)
		//{
		//	lvl.ReflectionProbe.SetActive(false);
		//}
		//_uGenLevels[index].ReflectionProbe.SetActive(true);
		//_uGenLevels[index].ReflectionProbe.GetComponent<ReflectionProbe>().bakedTexture = _uGenLevels[index].ProbeTexture;
	}
}
