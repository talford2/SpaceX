using UnityEngine;

public class Thruster : MonoBehaviour
{
    public LensFlare Flare;
    public float MaxFlareBrightness = 30f;

    public LineRenderer Trail;

    public void SetAmount(float amount)
    {
        Trail.SetPosition(1, amount*new Vector3(0, 0, 4f));
    }

    public void UpdateFlare()
    {
        var toCamera = Universe.Current.ViewPort.Shiftable.GetWorldPosition() - transform.position;
        const float theFactor = 2000f;
        var capFlareBright = MaxFlareBrightness / Mathf.Max(toCamera.sqrMagnitude / theFactor, 1f);
        Flare.brightness = 1f * capFlareBright;
    }
}
