using UnityEngine;
using System.Collections;

public class ProbeScript : MonoBehaviour
{
	private ReflectionProbe _probe;

	public RenderTexture _texture;

	public GameObject Background;


	public Material RenderedMaterialBackground;

	private int _renderId;

	private bool _hasRendered = false;

	void Awake()
	{
		_probe = GetComponent<ReflectionProbe>();

	}
	void Start()
	{
		_renderId = _probe.RenderProbe(_texture);

		//_texture = _probe.texture;
		//Background.SetActive(false);
		//Destroy(Background);


	}

	void Update()
	{
		if (!_hasRendered && _probe.IsFinishedRendering(_renderId))
		{
			Background.SetActive(false);


			RenderedMaterialBackground.SetColor("_TintColor", Color.red);
			RenderedMaterialBackground.SetColor("_Color", Color.red);
			RenderedMaterialBackground.SetTexture("_Cube", _probe.texture);


			Debug.Log("Rendered background");
			_hasRendered = transform;
		}
	}
}
