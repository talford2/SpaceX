﻿using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public Light DirectionLight;

	public List<LevelDefinition> Levels;

	public Material NextPortalMaterial;

	public Shader CubeMapShader;

    private EventGenerator _eventGenerator;

	private ReflectionProbe _reflectionProbe;

	private int _levelIndex = 0;

	private static LevelManager _current;

    private void Awake()
    {
        _current = this;
        _eventGenerator = GetComponent<EventGenerator>();

        var context = PlayerContext.Current;
        if (context != null)
        {
            _levelIndex = PlayerContext.Current.LevelIndex;
        }
    }

	private void Start()
	{
		// Reflection probe
		var g = new GameObject();
		g.name = "System Reflection Probe";
		_reflectionProbe = g.AddComponent<ReflectionProbe>();
		_reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
		_reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		_reflectionProbe.cullingMask = LayerMask.GetMask("Universe Backgrouund");
		_reflectionProbe.size = Vector3.one * (Universe.Current.CellSize + 100);
		_reflectionProbe.transform.position = Vector3.zero;

        foreach(var level in Levels)
        {
            level.SystemName = NameGenerator.GetRandomSystemName();
        }

		ChangeLevel(_levelIndex);
	}

	private void Update()
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

        DirectionLight.color = lvl.LightColour;
        DirectionLight.transform.forward = lvl.LightDirection;
        Debug.LogFormat("SUN DIR: ({0:f3}, {1:f3}, {2:f3})", lvl.LightDirection.x, lvl.LightDirection.y, lvl.LightDirection.z);
        //DirectionLight.transform.rotation = Quaternion.Euler(lvl.LightDirection);
        _reflectionProbe.RenderProbe();
        _eventGenerator.Clear();
        _eventGenerator.Generate(lvl.CellRadius, lvl.UniverseEvents, Random.Range(int.MinValue, int.MaxValue));
        _levelIndex = index;
    }

    public LevelDefinition GetLevel()
    {
        return Levels[_levelIndex];
    }

	public static LevelManager Current
	{
		get
		{
			return _current;
		}
	}
}
