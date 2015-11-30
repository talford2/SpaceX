using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniverseGenerator : MonoBehaviour
{
	public GameObject BackgroundContainer;

	public List<GameObject> Nebulas;

	public Color PrimaryColor = Color.red;

	public Color SecondaryColor = Color.yellow;

	public ReflectionProbe SceneRelfectionProbe;

	public Material BackgroundMaterial;

	public Material DustMaterial;

	public Material AddFogMaterial;

	private Light _sunLight;

	public GameObject SunObject;
	private GameObject _sunObj;

	public int MinNebulas = 30;

	public int MaxNubulas = 40;

	public bool UseRandomColours = false;

	public Camera BackgroundCamera;

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

	void Awake()
	{
		RandomiseUniverse();
	}

	void Start()
	{

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
			Destroy(trans.gameObject);
		}

		_sunObj = Instantiate<GameObject>(SunObject);
		_sunLight = _sunObj.GetComponentInChildren<Light>();
		_sunObj.transform.SetParent(BackgroundContainer.transform);

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
		bg.V *= Random.Range(0.05f, 0.2f);
		BackgroundCamera.backgroundColor = bg.GetColor();

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
			gm.transform.SetParent(BackgroundContainer.transform);
			gm.layer = LayerMask.NameToLayer("Universe Background");
            gm.transform.localPosition = Vector3.zero;

			var randC = HSVColor.FromColor(Utility.GetRandomColor(PrimaryColor, SecondaryColor, 0.2f));
			randC.V *= 0.1f;
			gm.GetComponent<Renderer>().material.SetColor("_Color", randC.GetColor());
			//gm.GetComponent<Renderer>().material.SetColor("_TintColor", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f, 255f));
		}

		// Planets
		int totalPlanets = Random.Range(MinPlanets, MaxPlanets);
		for (var i = 0; i < totalPlanets; i++)
		{
			var pl = Instantiate<GameObject>(Planets[Random.Range(0, Planets.Count)]);
			pl.transform.SetParent(BackgroundContainer.transform);
			pl.transform.localPosition = Random.onUnitSphere * 800f;
			pl.transform.rotation = Random.rotation;
			pl.layer = LayerMask.NameToLayer("Universe Background");
			pl.transform.localScale = Random.Range(20f, 100f) * Vector3.one;
		}

		_sunObj.transform.localPosition = Random.onUnitSphere * 1000f;
		_sunObj.transform.forward = Vector3.zero - _sunObj.transform.localPosition;

		var sunColor = HSVColor.FromColor(bgColor);
		sunColor.V = 1f;
		sunColor.S *= Random.Range(0.1f, 1f);
		_sunLight.color = sunColor.GetColor();

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
		}

		SceneRelfectionProbe.backgroundColor = bg.GetColor();
		SceneRelfectionProbe.RenderProbe();
	}
}