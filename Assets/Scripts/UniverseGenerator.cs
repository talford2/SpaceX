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

	void Awake()
	{
		PrimaryColor = Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.6f, 1f, 1f);
		SecondaryColor = Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.4f, 0.6f, 1f, 1f);

		var bgColor = Utility.GetRandomColor(PrimaryColor, SecondaryColor);
		var ccc = HSVColor.FromColor(bgColor);
		BackgroundMaterial.SetColor("_Tint", bgColor);

		var yyy = HSVColor.FromColor(bgColor);
		yyy.V *= 0.5f;
		yyy.S *= 0.5f;
		DustMaterial.SetColor("_Color", yyy.GetColor());

		var kkk = HSVColor.FromColor(bgColor);
		kkk.S *= 0.5f;
		kkk.V *= 0.4f;
		AddFogMaterial.SetColor("_Color", kkk.GetColor());

		int totalNebula = Random.Range(30, 40);

		for (var i = 0; i < totalNebula; i++)
		{
			var gm = Instantiate<GameObject>(Nebulas[Random.Range(0, Nebulas.Count)]);
			gm.transform.rotation = Random.rotationUniform;
			gm.transform.SetParent(BackgroundContainer.transform);

			gm.GetComponent<Renderer>().material.SetColor("_Color", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 0.1f, 0.5f));


			//gm.GetComponent<Renderer>().material.SetColor("_TintColor", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f, 255f));
		}

		SunObject.transform.position = Random.onUnitSphere * 1000f;
		SunLight.transform.rotation = Quaternion.LookRotation(SunObject.transform.position * -1);

		ccc.V = 1;
		ccc.S = Mathf.Min(ccc.S, 0.2f);
		SunLight.color = ccc.GetColor();

	}

	void Start()
	{
		SceneRelfectionProbe.RenderProbe();
	}
}
