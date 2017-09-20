using UnityEngine;

public class CoverScreen : MonoBehaviour
{
    public CanvasGroup CoverGroup;

    public delegate void OnCoverScreenFadeComplete();

    public OnCoverScreenFadeComplete OnFadeComplete;

    private bool isFadeOut;
    private float coverTime = 0.5f;
    private float coverCooldown;

    private void Update()
    {
        if (coverCooldown >= 0f)
        {
            coverCooldown -= Time.deltaTime;
            var fraction = Mathf.Clamp01(coverCooldown / coverTime);
            CoverGroup.alpha = isFadeOut ? fraction : 1f - fraction;
            if (coverCooldown < 0f)
            {
                CoverGroup.alpha = isFadeOut ? 0f : 1f;
                if (OnFadeComplete != null)
                    OnFadeComplete();
            }
        }
    }

    public void TriggerFadeIn(float duration = 0.5f)
    {
        isFadeOut = false;
        coverTime = duration;
        coverCooldown = coverTime;
    }

    public void TriggerFadeOut(float duration = 0.5f)
    {
        isFadeOut = true;
        coverTime = duration;
        coverCooldown = coverTime;
    }
}
