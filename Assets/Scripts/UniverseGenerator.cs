using System.Collections.Generic;
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

	public Light SunLight;

	public GameObject SunObject;

	public int MinNebulas = 30;

	public int MaxNubulas = 40;

	public bool USeRandomColours = false;

	public Camera BackgroundCamera;

	void Awake()
	{
		RandomiseUniverse();
	}

	void Start()
	{
		SceneRelfectionProbe.RenderProbe();
	}

	private void RandomiseUniverse()
	{
		if (USeRandomColours)
		{
			var h = HSVColor.FromColor(Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.7f, 1f, 1f));
			PrimaryColor = h.GetColor();

			var h2 = HSVColor.FromColor(Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.7f, 1f, 1f));
			h2.H = h.H + Random.Range(0.2f, 0.8f);
			while (h2.H > 1)
			{
				h2.H -= 1;
			}

			SecondaryColor = h2.GetColor(); //Random.ColorHSV(0f, 1f, 0.5f, 0.95f, 0.4f, 0.8f, 1f, 1f);
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


		int totalNebula = Random.Range(MinNebulas, MaxNubulas);

		for (var i = 0; i < totalNebula; i++)
		{
			var gm = Instantiate<GameObject>(Nebulas[Random.Range(0, Nebulas.Count)]);
			gm.transform.rotation = Random.rotationUniform;
			gm.transform.SetParent(BackgroundContainer.transform);

			var randC = HSVColor.FromColor(Utility.GetRandomColor(PrimaryColor, SecondaryColor, 0.2f));
			randC.V *= 0.1f;
			//randC.S *= 1.3f;
			gm.GetComponent<Renderer>().material.SetColor("_Color", randC.GetColor());
			//gm.GetComponent<Renderer>().material.SetColor("_TintColor", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f, 255f));
		}

		SunObject.transform.position = Random.onUnitSphere * 1000f;
		SunLight.transform.rotation = Quaternion.LookRotation(SunObject.transform.position * -1);

		var sunColor = HSVColor.FromColor(bgColor);
		sunColor.V = 1f;
		sunColor.S *= Random.Range(0.1f, 1f);
		SunLight.color = sunColor.GetColor();
	}
}