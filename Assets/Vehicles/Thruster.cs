using UnityEngine;

public class Thruster : MonoBehaviour
{
    public LensFlare Flare;
    public float MaxFlareBrightness = 30f;
    public float MaxLength = 100f;

    public LineRenderer Trail;

    private Vector3 tailPoint;
    private Vector3 targetTail;

    public void SetAmount(float amount)
    {
        if (amount > 0)
        {
            targetTail = amount*new Vector3(0, 0, MaxLength);
        }
        else
        {
            targetTail = Vector3.zero;
        }
    }

    public void UpdateFlare()
    {
        tailPoint = Vector3.Lerp(tailPoint, targetTail, 2f*Time.deltaTime);
        Trail.SetPosition(1, tailPoint);

        var toCamera = transform.position - Universe.Current.ViewPort.Shiftable.GetWorldPosition();
        const float theFactor = 2000f;
        var capFlareBright = MaxFlareBrightness / Mathf.Max(toCamera.sqrMagnitude / theFactor, 1f);
        Flare.brightness = 1f * capFlareBright;
    }

    public void SetVisibility(bool value)
    {
        Trail.enabled = value;
        Flare.enabled = value;
    }
}
