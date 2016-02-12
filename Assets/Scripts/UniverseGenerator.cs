using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UniverseGenerator : MonoBehaviour
{
	#region Private Members

	private GameObject _universeObj;

	private Transform _parent;

	private Camera _renderCamera;

	private GameObject _reflectionProbeObj;

	private bool _hasGenerated = false;

	private Material _mat;

	public bool HasGenerated
	{
		get
		{
			return _hasGenerated;
		}
	}

	#endregion

	#region Public Variables

	public string BackgroundLayerName = "BackgroundLayer";

	public int FlatResolution = 2048;

	public Shader BaseShader;

	public Shader CubemapShader;

	public Color BackgroundColor = Color.black;

	public List<ScatterSettings> ScatterObjects;

	// Sun
	public Light SunLight;

	public Texture SunTexture;

	public GameObject SunModel;

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
		Generate();
		
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.U))
		{
			Generate();
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
		Generate();
		Flatten();
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

		_renderCamera.clearFlags = CameraClearFlags.Skybox;
		_renderCamera.enabled = false;

		if (_reflectionProbeObj != null)
		{
			Destroy(_reflectionProbeObj);
		}

		// Reflection probe
		_reflectionProbeObj = new GameObject();
		_reflectionProbeObj.name = "Reflection Probe";
		var sceneRelfectionProbe = _reflectionProbeObj.AddComponent<ReflectionProbe>();
		sceneRelfectionProbe.size = Vector3.one * 1500f;
		sceneRelfectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;

		//sceneRelfectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
		//sceneRelfectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		//sceneRelfectionProbe.customBakedTexture = renderTexture;

		sceneRelfectionProbe.backgroundColor = Color.red;
		sceneRelfectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
		sceneRelfectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		sceneRelfectionProbe.cullingMask = LayerMask.GetMask(BackgroundLayerName);
		//sceneRelfectionProbe.resolution = 2048;
		sceneRelfectionProbe.resolution = 512;
		var rendId = sceneRelfectionProbe.RenderProbe();
		sceneRelfectionProbe.hdr = true;
		
		//_parent.gameObject.SetActive(false);
		Destroy(_parent.gameObject);
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