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

	public Light SunLight;

	public GameObject SunObject;

	void Awake()
	{
		PrimaryColor = Utility.GetRandomColor(0.3f);
		SecondaryColor = Utility.GetRandomColor(0.7f);

		var bgColor = Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f);
		BackgroundMaterial.SetColor("_Tint", bgColor);
		DustMaterial.SetColor("_Color", bgColor);

		int totalNebula = Random.Range(10, 15);

		for (var i = 0; i < totalNebula; i++)
		{
			var gm = Instantiate<GameObject>(Nebulas[Random.Range(0, Nebulas.Count)]);
			gm.transform.rotation = Random.rotationUniform;
			gm.transform.SetParent(BackgroundContainer.transform);

			//gm.GetComponent<Renderer>().material.SetColor("_Color", Utility.GetRandomColor(PrimaryColor, SecondaryColor));
			gm.GetComponent<Renderer>().material.SetColor("_TintColor", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f));
		}

		SunObject.transform.position = Random.onUnitSphere * 1000f;
		SunLight.transform.rotation = Quaternion.LookRotation(SunObject.transform.position * -1);

		SunLight.color = Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f);

	}

	void Start()
	{
		SceneRelfectionProbe.RenderProbe();
	}
}
