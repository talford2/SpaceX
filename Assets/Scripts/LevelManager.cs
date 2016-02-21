using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public Light DirectionLight;

	public List<Level> Levels;

	public Material NextPortalMaterial;

	public Shader CubeMapShader;

	private ReflectionProbe _reflectionProbe;

	private int _levelIndex = 0;

	private static LevelManager _current;

	void Start()
	{
		_current = this;

		// Reflection probe
		var g = new GameObject();
		g.name = "System Reflection Probe";
		_reflectionProbe = g.AddComponent<ReflectionProbe>();
		_reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
		_reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		_reflectionProbe.cullingMask = LayerMask.GetMask("Universe Backgrouund");
		_reflectionProbe.size = Vector3.one * (Universe.Current.CellSize + 100);
		_reflectionProbe.transform.position = Vector3.zero;

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

	public void ChangeLevel(int index)
	{
		var lvl = Levels[index];
		RenderSettings.skybox = lvl.Material;

		if (NextPortalMaterial != null)
		{
			if (index < Levels.Count - 1)
			{
				NextPortalMaterial.SetTexture("_Cube", Levels[index + 1].Material.GetTexture("_Tex"));
			}
			else
			{
				NextPortalMaterial.SetTexture("_Cube", Levels[0].Material.GetTexture("_Tex"));
			}
		}

		DirectionLight.color = lvl.LighColour;
		DirectionLight.transform.forward = lvl.LightDirection;
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
