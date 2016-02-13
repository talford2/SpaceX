using UnityEngine;

public class DecalFadeOut : MonoBehaviour
{
	public Projector ProjectorObject;

	private Material _mat;

	public float FadeoutTime = 2;

	private float _coolDown = 0;

	void Awake()
	{
		_mat = Instantiate<Material>(ProjectorObject.material);
		ProjectorObject.material = _mat;
		_coolDown = FadeoutTime;
	}

	void Update()
	{
		var frac = _coolDown / FadeoutTime;
		_mat.SetFloat("_Alpha", frac);
		_coolDown -= Time.deltaTime;
	}
}
