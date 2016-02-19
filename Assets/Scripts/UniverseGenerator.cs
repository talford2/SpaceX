﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UniverseGenerator : MonoBehaviour
{
	#region Private Members

	private GameObject _universeObj;

	private Transform _parent;

	private Camera _renderCamera;

	private bool _hasGenerated = false;

	private Material _mat;

	private Vector3 _sunDirection;

	private bool _hasStartedGenerating = false;

	private RenderTexture _probeRenderTexture;

	#endregion

	#region Public Variables

	public string BackgroundLayerName = "BackgroundLayer";

	public int FlatResolution = 2048;

	public Shader BaseShader;

	public Shader CubemapShader;

	public Color BackgroundColor = Color.black;

	public List<ScatterSettings> ScatterObjects;

	public bool HasGenerated
	{
		get
		{
			return _hasGenerated;
		}
	}

	// Sun
	public Light SunLight;

	public Texture SunTexture;

	public GameObject SunModel;

	public event System.Action FinishedRendering;

	#endregion

	#region Public Properties

	public Material Background
	{
		get
		{
			return _mat;
		}
	}

	public Vector3 SunDirection
	{
		get
		{
			return _sunDirection;
		}
	}

	public Texture ProbeTexture
	{
		get
		{
			return _probeRenderTexture;
		}
	}

	#endregion

	public UniverseGenerator()
	{
		if (ScatterObjects == null)
		{
			ScatterObjects = new List<ScatterSettings>();
		}
	}

	void Start()
	{
		if (ScatterObjects == null)
		{
			ScatterObjects = new List<ScatterSettings>();
		}
		//Generate();
	}

	void LateUpdate()
	{
		Debug.Log("I was called!");

		if (_hasStartedGenerating)
		{
			Debug.Log("Done: " + gameObject.name);
			DestroyImmediate(_parent.gameObject);
			_hasGenerated = true;
			_hasStartedGenerating = false;
			if (FinishedRendering != null)
			{
				FinishedRendering();
			}
		}
	}

	#region Public Methods

	public void Generate()
	{
		Destroy(_universeObj);
		_universeObj = new GameObject("UniverseObject");
		_parent = _universeObj.transform;

		// Construct Camera
		var camObj = new GameObject("BackgroundCamera");
		camObj.transform.parent = _parent;
		_renderCamera = camObj.AddComponent<Camera>();
		// VERY IMPORTANT - must set clear to color before creating universe, then set to skybox after clearing
		_renderCamera.clearFlags = CameraClearFlags.Color;
		_renderCamera.renderingPath = RenderingPath.DeferredShading;
		_renderCamera.hdr = true;
		_renderCamera.farClipPlane = 20000;
		_renderCamera.cullingMask = LayerMask.GetMask(BackgroundLayerName);
		_renderCamera.backgroundColor = BackgroundColor;

		// Sun
		var sunObj = Instantiate<GameObject>(SunModel);
		sunObj.transform.SetParent(_parent);
		sunObj.layer = LayerMask.NameToLayer(BackgroundLayerName);
		sunObj.transform.position = Random.onUnitSphere * 10000;
		sunObj.transform.localScale = Vector3.one * 2000;
		sunObj.GetComponent<Renderer>().material = CreateMaterial(SunTexture, Color.white);
		sunObj.transform.rotation = LookAtWithRandomTwist(sunObj.transform.position, Vector3.zero);
		SunLight.transform.position = sunObj.transform.position;
		SunLight.transform.forward = Vector3.zero - sunObj.transform.localPosition;
		_sunDirection = SunLight.transform.forward;

		foreach (var sg in ScatterObjects)
		{
			if (sg.IsActive)
			{
				Scatter(sg);
			}
		}

		Flatten();
		_hasGenerated = true;
	}

	public Material GetMaterial()
	{
		Debug.Log("Get material : " + gameObject.name);
		Generate();
		Flatten();
		_hasStartedGenerating = true;
		return _mat;
	}

	public void Flatten()
	{
		var renderTexture = new RenderTexture(FlatResolution, FlatResolution, 24);
		renderTexture.wrapMode = TextureWrapMode.Repeat;
		renderTexture.antiAliasing = 2;
		renderTexture.anisoLevel = 9;
		renderTexture.filterMode = FilterMode.Trilinear;
		renderTexture.generateMips = false;
		renderTexture.isCubemap = true;

		_mat = new Material(CubemapShader);
		_mat.SetTexture("_Tex", renderTexture);

		RenderSettings.skybox = _mat;

		_renderCamera.RenderToCubemap(renderTexture);

		// Reflection probe
		_probeRenderTexture = new RenderTexture(256, 256, 24);
		_probeRenderTexture.wrapMode = TextureWrapMode.Repeat;
		_probeRenderTexture.antiAliasing = 2;
		_probeRenderTexture.anisoLevel = 9;
		_probeRenderTexture.filterMode = FilterMode.Trilinear;
		_probeRenderTexture.generateMips = false;
		_probeRenderTexture.isCubemap = true;
		_renderCamera.RenderToCubemap(_probeRenderTexture);

		_renderCamera.enabled = false;
	}

	#endregion

	#region Private Methods

	private void Scatter(ScatterSettings settings)
	{
		var count = Random.Range(settings.CountMin, settings.CountMax);
		for (var i = 0; i < count; i++)
		{
			var model = Instantiate<GameObject>(settings.Model);
			model.layer = LayerMask.NameToLayer(BackgroundLayerName);

			if (settings.RadiusMax == 0 && settings.RadiusMin == 0)
			{
				model.transform.position = Vector3.zero;
				model.transform.rotation = Random.rotation;
			}
			else {
				model.transform.position = Random.onUnitSphere * Random.Range(settings.RadiusMin, settings.RadiusMax);

				if (settings.LookAtCenter)
				{
					model.transform.rotation = LookAtWithRandomTwist(model.transform.position, Vector3.zero);
				}
				else
				{
					model.transform.rotation = Random.rotation;
				}
			}

			model.transform.localScale = Vector3.one * Random.Range(settings.ScaleMin, settings.ScaleMax);
			model.transform.SetParent(_parent);

			if (settings.UseMaterials)
			{
				model.GetComponent<Renderer>().material = settings.Materials[Random.Range(0, settings.Materials.Count)];
			}
			else {
				if (settings.Textures != null && settings.Textures.Any())
				{
					var tex = settings.Textures[Random.Range(0, settings.Textures.Count)];
					var colr = settings.Colors[Random.Range(0, settings.Colors.Count)].GetRandom();
					model.GetComponent<Renderer>().material = CreateMaterial(tex, colr);
				}
			}
		}
	}

	private Quaternion LookAtWithRandomTwist(Vector3 positon, Vector3 target)
	{
		var relativeForward = target - positon;
		var lookat = Quaternion.LookRotation(relativeForward);

		// This isn't right yet
		//lookat = Quaternion.AngleAxis(Random.Range(0f, 360f), forwardS);

		return lookat;
	}

	private Material CreateMaterial(Texture tex, Color color)
	{
		var mat = new Material(BaseShader);
		mat.SetTexture("_MainTex", tex);
		mat.SetColor("_Color", color);
		return mat;
	}

	#endregion
}

[System.Serializable]
public class ScatterSettings
{
	public string Name;

	public bool IsActive;

	public int CountMin;

	public int CountMax;

	public float ScaleMin;

	public float ScaleMax;

	public float RadiusMin;

	public float RadiusMax;

	public GameObject Model;

	public bool LookAtCenter;

	public List<Texture> Textures;

	public List<ColorRange> Colors;

	public bool UseMaterials;

	public List<Material> Materials;

	public ScatterSettings()
	{
		IsActive = true;
		Colors = new List<ColorRange> { new ColorRange() };
	}
}

[System.Serializable]
public class ColorRange
{
	public Color Color1;

	public Color Color2;

	public ColorRange()
	{
		Color1 = Color.white;
		Color2 = Color.white;
	}

	public Color GetRandom()
	{
		return Utility.GetRandomColor(Color1, Color2);
	}
}