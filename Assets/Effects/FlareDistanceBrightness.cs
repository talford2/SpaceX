using UnityEngine;

[RequireComponent(typeof(LensFlare))]
public class FlareDistanceBrightness : MonoBehaviour
{
	public float MaxFlareBrightness = 30f;
	private LensFlare _flare;
	private const float _theFactor = 2000f;

	private void Awake()
	{
		_flare = GetComponent<LensFlare>();
	}

	private void LateUpdate()
	{
		_flare.brightness = 1f * MaxFlareBrightness / Mathf.Max((transform.position - Universe.Current.ViewPort.Shiftable.GetWorldPosition()).sqrMagnitude / _theFactor, 1f);
	}

	public void SetVisible(bool value)
	{
		_flare.enabled = value;
	}
}
