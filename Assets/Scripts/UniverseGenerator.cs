using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UniverseGenerator : MonoBehaviour
{
	public Color PrimaryColor = Color.red;
	public Color SecondaryColor = Color.yellow;
	public GameObject BackgroundContainer;
	public ReflectionProbe SceneRelfectionProbe;
	public GameObject SunObject;
	public bool UseRandomColours = false;
	public Camera BackgroundCamera;
	public bool FlattenToTexture = false;

	// Cube map stuff
	public RenderTexture BackgroundGenCubmap;
	public Material BackgroundGenMaterial;
	public int FlatResolution = 4096;

	// Materials
	public Material BackgroundMaterial;
	public Material DustMaterial;
	public Material AddFogMaterial;

	public GameObject UniverseRing;

	// Nebulas
	public int MinNebulas = 30;
	public int MaxNubulas = 40;
	public float NebulaBrightnessMultiplier = 0.1f;
	public List<GameObject> Nebulas;

	// Planets
	public int MinPlanets = 4;
	public int MaxPlanets = 10;
	public List<GameObject> Planets;

	// Starfield
	public List<GameObject> StarPrefabs;
	public float MinSize;
	public float MaxSize;
	public float Radius;
	public int Count;

	// Universe Events
	public int CellRadius = 10;
	public List<UniverseEventCount> UniverseEvents;

	private Light _sunLight;
	private GameObject _sunObj;

	private List<GameObject> _destroyAfterGeneration;

	void Awake()
	{
		_destroyAfterGeneration = new List<GameObject>();

		if (BackgroundContainer == null)
		{
			BackgroundContainer = new GameObject();
			BackgroundContainer.name = "Universe Container";
		}

		if (SceneRelfectionProbe == null)
		{
			var reflectionProbe = new GameObject();
			reflectionProbe.name = "Reflection Probe";
			SceneRelfectionProbe = reflectionProbe.AddComponent<ReflectionProbe>();
			SceneRelfectionProbe.size = Vector3.one * 1500f;
			SceneRelfectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
			SceneRelfectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
			SceneRelfectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
		}
	}

	public void Start()
	{
		RandomiseUniverse();
		RandomiseUniverseEvents();
	}

	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.U))
		{
			RandomiseUniverse();
		}
	}

	private void RandomiseUniverse()
	{
		foreach (Transform trans in BackgroundContainer.transform)
		{
			DestroyImmediate(trans.gameObject);
		}

		_sunObj = Instantiate<GameObject>(SunObject);
		_sunObj.transform.SetParent(BackgroundContainer.transform);

		_sunLight = _sunObj.GetComponentInChildren<Light>();

		if (UseRandomColours)
		{
			var h = HSVColor.FromColor(Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.7f, 1f, 1f));
			PrimaryColor = h.GetColor();

			var h2 = HSVColor.FromColor(Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.7f, 1f, 1f));
			h2.H = h.H + Random.Range(0.2f, 0.8f);
			while (h2.H > 1)
			{
				h2.H -= 1;
			}
			SecondaryColor = h2.GetColor();
		}

		var bgColor = Utility.GetRandomColor(PrimaryColor, SecondaryColor);
		BackgroundMaterial.SetColor("_Tint", bgColor);

		var bg = HSVColor.FromColor(bgColor);
		//bg.V *= Random.Range(0.05f, 0.2f);
		//bg.V *= Random.Range(0.05f, 0.5f);
		bg.V = Random.Range(0.02f, 0.15f);

		BackgroundCamera.clearFlags = CameraClearFlags.Color;
		BackgroundCamera.backgroundColor = bg.GetColor();


		// Draw universe ring
		var ring = Instantiate<GameObject>(UniverseRing);
		ring.transform.localScale *= 1000F;
		ring.transform.position = Vector3.zero;
		ring.transform.rotation = Random.rotation;
		//ring.GetComponent<Renderer>().material.SetColor("_Color", bg.GetColor());

		var ringColor = new HSVColor(Utility.GetRandomColor(PrimaryColor, SecondaryColor));
		ringColor.A = Random.Range(0.03f, 0.15f);
		ring.GetComponent<Renderer>().material.SetColor("_Color", ringColor.GetColor());
		_destroyAfterGeneration.Add(ring);

		var fogColor = HSVColor.FromColor(bgColor);
		fogColor.S *= 0.5f;
		fogColor.V *= 0.3f;
		AddFogMaterial.SetColor("_Color", fogColor.GetColor());

		var dustColor = HSVColor.FromColor(bgColor);
		dustColor.V *= 0.5f;
		dustColor.S *= 0.5f;
		DustMaterial.SetColor("_Color", dustColor.GetColor());

		// Nebula
		int totalNebula = Random.Range(MinNebulas, MaxNubulas);
		for (var i = 0; i < totalNebula; i++)
		{
			var gm = Instantiate<GameObject>(Nebulas[Random.Range(0, Nebulas.Count)]);
			gm.transform.rotation = Random.rotationUniform;
			gm.transform.localScale = Vector3.one * Random.Range(0.6f, 1.3f);
			gm.transform.SetParent(BackgroundContainer.transform);
			gm.layer = LayerMask.NameToLayer("Universe Background");
			gm.transform.localPosition = Vector3.zero;

			//var randC = HSVColor.FromColor(Utility.GetRandomColor(PrimaryColor, SecondaryColor, 0.3f));
			var randC = HSVColor.FromColor(Utility.GetRandomColor(PrimaryColor, SecondaryColor, 0.8f));
			//randC.V *= NebulaBrightnessMultiplier;
			//randC.V = 0.05f;
			randC.V = Random.Range(0.01f, 0.05f);
			randC.S = 0.9f;

			gm.GetComponent<Renderer>().material.SetColor("_Color", randC.GetColor());

			_destroyAfterGeneration.Add(gm);
			//gm.GetComponent<Renderer>().material.SetColor("_TintColor", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f, 255f));
		}

		// Planets
		GeneratePlanets(bg);

		_sunObj.transform.localPosition = Random.onUnitSphere * 1000f;
		_sunObj.transform.forward = Vector3.zero - _sunObj.transform.localPosition;

		var sunColor = HSVColor.FromColor(bgColor);
		sunColor.V = 1f;
		sunColor.S *= Random.Range(0.1f, 0.5f);
		_sunLight.color = sunColor.GetColor();

		if (StarPrefabs.Count > 0)
		{
			var c = new Color(1f, 1f, 1f, Random.Range(0.2f, 1f));
			StarPrefabs.First().GetComponentInChildren<Renderer>().sharedMaterial.SetColor("_Color", c);
		}

		// Stars
		for (var i = 0; i < Count; i++)
		{
			var position = Radius * Random.onUnitSphere;
			var star = Utility.InstantiateInParent(StarPrefabs[Random.Range(0, StarPrefabs.Count)], transform);

			Utility.SetLayerRecursively(star, LayerMask.NameToLayer("Universe Background"));
			star.transform.localScale = Vector3.one * Random.Range(MinSize, MaxSize);
			star.transform.SetParent(BackgroundContainer.transform);
			star.transform.localPosition = position;
			star.transform.LookAt(Camera.main.transform, transform.up);
			star.transform.rotation *= Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);

			_destroyAfterGeneration.Add(star);
		}

		SceneRelfectionProbe.backgroundColor = bg.GetColor();
		SceneRelfectionProbe.RenderProbe();

		if (FlattenToTexture)
		{
			BackgroundSnapshop();
		}
	}

	private void GeneratePlanets(HSVColor backgroundColor)
	{
		// Planets
		int totalPlanets = Random.Range(MinPlanets, MaxPlanets);
		for (var i = 0; i < totalPlanets; i++)
		{
			var pl = Instantiate<GameObject>(Planets[Random.Range(0, Planets.Count)]);
			pl.transform.SetParent(BackgroundContainer.transform);

			pl.transform.localScale = Random.Range(0.1f, 50f) * Vector3.one;
			//var planetPos = Random.onUnitSphere * Random.Range(400f, 800f);
			var planetPos = Random.onUnitSphere * Random.Range(200f, 400f);

			pl.transform.localPosition = planetPos;

			pl.transform.rotation = Random.rotation;
			pl.layer = LayerMask.NameToLayer("Universe Background");

			var mat = pl.GetComponent<Renderer>().material;
			backgroundColor.V = 0.15f;
			backgroundColor.A = 0.5f;

			mat.EnableKeyword("_EMISSION");
			mat.SetColor("_EmissionColor", backgroundColor.GetColor());

			mat.SetColor("_EnvironmentColor", backgroundColor.GetColor());

			pl.GetComponent<Renderer>().material = mat;

			_destroyAfterGeneration.Add(pl);
		}
	}

	private void RandomiseUniverseEvents()
	{
		foreach (var ue in UniverseEvents)
		{
			for (var i = 0; i < ue.Count; i++)
			{
				var eventObj = Instantiate(ue.Prefab).GetComponent<UniverseEvent>();
				var shifter = eventObj.Shiftable;
				eventObj.transform.rotation = Random.rotation;
				shifter.UniverseCellIndex = new CellIndex(Random.insideUnitSphere * CellRadius);
				shifter.CellLocalPosition = Utility.RandomInsideCube * Universe.Current.CellSize;
				Universe.Current.UniverseEvents.Add(eventObj);
			}
		}
	}

	private void BackgroundSnapshop()
	{
		var renText = new RenderTexture(FlatResolution, FlatResolution, 24);

		renText.wrapMode = TextureWrapMode.Repeat;
		renText.antiAliasing = 2;
		renText.anisoLevel = 9;
		renText.filterMode = FilterMode.Trilinear;
		renText.generateMips = false;
		renText.isCubemap = true;

		BackgroundGenMaterial.SetTexture("_Tex", renText);

		RenderSettings.skybox = BackgroundGenMaterial;

		//_sunObj.SetActive(false);
		//Destroy(_sunObj);
		BackgroundCamera.RenderToCubemap(renText, 63);

		//_sunObj.SetActive(true);

		// Save to file
		//RenderTexture.active = renText;
		//var new2dTex = new Texture2D(FlatResolution * 1, FlatResolution * 1);
		//new2dTex.ReadPixels(new Rect(0, 0, FlatResolution * 1, FlatResolution * 1), 0, 0);
		//var bytesCode = new2dTex.EncodeToPNG();
		//File.WriteAllBytes(@"C:\Test\test.png", bytesCode);

		foreach (var destroyerable in _destroyAfterGeneration)
		{
			Destroy(destroyerable);
		}
		BackgroundCamera.clearFlags = CameraClearFlags.Skybox;
		SceneRelfectionProbe.RenderProbe();
	}
}

[System.Serializable]
public class UniverseEventCount
{
	public GameObject Prefab;

	public int Count;
}