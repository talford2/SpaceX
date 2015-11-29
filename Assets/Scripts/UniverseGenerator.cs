using System.Collections.Generic;
using UnityEngine;

public class UniverseGenerator : MonoBehaviour
{
	public GameObject BackgroundContainer;

	public List<GameObject> BackgroundObject;

	public Color PrimaryColor = Color.red;

	public Color SecondaryColor = Color.yellow;

	public ReflectionProbe SceneRelfectionProbe;

	public Material BackgroundMaterial;

	public Light Sun;

	void Start()
	{
		PrimaryColor = Utility.GetRandomColor(0.3f);
		SecondaryColor = Utility.GetRandomColor(0.7f);

		BackgroundMaterial.SetColor("_Tint", Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f));

		int totalNebula = Random.Range(10, 15);

		for (var i = 0; i < totalNebula; i++)
		{
			var gm = Instantiate<GameObject>(BackgroundObject[Random.Range(0, BackgroundObject.Count)]);
			gm.transform.rotation = Random.rotationUniform;
			gm.transform.SetParent(BackgroundContainer.transform);

			gm.GetComponent<Renderer>().material.SetColor("_Color", Utility.GetRandomColor(PrimaryColor, SecondaryColor));
		}

		Sun.color = Utility.GetRandomColor(PrimaryColor, SecondaryColor, 1f);

		SceneRelfectionProbe.RenderProbe();
	}
}
