using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
	public GameObject MuzzleBase;

	public LineRenderer Line;

	public float LineLength = 5f;

	public float EffectTime = 0.2f;
	
	public AnimationCurve ScaleOverTime;

	public AnimationCurve AlphaOverTime;

    public Light FlashLight;

	private Renderer _muzzleBaseRenderer;

	private float _effectCooldown;

	void Awake()
	{
		_muzzleBaseRenderer = MuzzleBase.GetComponent<Renderer>();
		_muzzleBaseRenderer.enabled = false;
		Line.enabled = false;
	    FlashLight.enabled = false;
	}

	void Update()
	{
		if (_effectCooldown > 0)
		{
			_effectCooldown -= Time.deltaTime;
			var frac = 1 - _effectCooldown / EffectTime;

			var scale = ScaleOverTime.Evaluate(frac);
			var alpha = AlphaOverTime.Evaluate(frac);
			MuzzleBase.transform.localScale = Vector3.one * scale;

		    //_muzzleBaseRenderer.material.color = Utility.SetColorAlpha(_muzzleBaseRenderer.material.color, alpha);
		    //Line.material.color = Utility.SetColorAlpha(Line.material.color, alpha);

			Line.SetPosition(1, Vector3.forward * scale * LineLength);

		    FlashLight.intensity = 5f*frac;
		}
		else
		{
			_muzzleBaseRenderer.enabled = false;
			Line.enabled = false;
		    FlashLight.enabled = false;
		}
	}

	public void Flash()
	{
		var scale = ScaleOverTime.Evaluate(0f);

		_muzzleBaseRenderer.enabled = true;
		MuzzleBase.transform.localScale = Vector3.one * scale;
	    var alpha = AlphaOverTime.Evaluate(0f);

        //_muzzleBaseRenderer.material.color = Utility.SetColorAlpha(_muzzleBaseRenderer.material.color, alpha);

		Line.enabled = true;
		Line.SetPosition(0, Vector3.zero);
		Line.SetPosition(1, Vector3.forward * scale * LineLength);
		//Line.material.color = Utility.SetColorAlpha(Line.material.color, alpha);

	    FlashLight.enabled = true;

		_effectCooldown = EffectTime;
		MuzzleBase.transform.rotation *= Quaternion.AngleAxis(Random.Range(-180, 180f), Vector3.forward);
	}
}
