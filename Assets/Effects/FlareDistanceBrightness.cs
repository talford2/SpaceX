using UnityEngine;

[RequireComponent(typeof (LensFlare))]
public class FlareDistanceBrightness : MonoBehaviour
{
    public float MaxFlareBrightness = 30f;
    private LensFlare flare;

    private void Awake()
    {
        flare = GetComponent<LensFlare>();
    }

    private void LateUpdate()
    {
        var toCamera = transform.position - Universe.Current.ViewPort.Shiftable.GetWorldPosition();
        const float theFactor = 2000f;
        var capFlareBright = MaxFlareBrightness/Mathf.Max(toCamera.sqrMagnitude/theFactor, 1f);
        flare.brightness = 1f*capFlareBright;
    }

    public void SetVisible(bool value)
    {
        flare.enabled = value;
    }
}
