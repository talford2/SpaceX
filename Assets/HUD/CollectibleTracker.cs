using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleTracker : Tracker
{
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCursorImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;

    private Vector2 screenCentre;
    private Rect screenBounds;
    private Image imageInstance;

    private Sprite trackerSprite;
    private Sprite farTrackerSprite;
    private Sprite veryFarTrackerSprite;
    private Sprite arrowSprite;

    private bool isVisible;
    private bool isFading;

    private void Awake()
    {
        screenCentre = new Vector3(0.5f*Screen.width, 0.5f*Screen.height);
        var boundaryPadding = 20f;
        screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f*boundaryPadding, Screen.height - 2f*boundaryPadding);
    }

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);

        trackerSprite = Sprite.Create(TrackerCursorImage, new Rect(0, 0, TrackerCursorImage.width, TrackerCursorImage.height), Vector2.zero);
        farTrackerSprite = Sprite.Create(FarTrackerCursorImage, new Rect(0, 0, FarTrackerCursorImage.width, FarTrackerCursorImage.height), Vector2.zero);
        veryFarTrackerSprite = Sprite.Create(VeryFarTrackerCursorImage, new Rect(0, 0, VeryFarTrackerCursorImage.width, VeryFarTrackerCursorImage.height), Vector2.zero);
        arrowSprite = Sprite.Create(ArrowCursorImage, new Rect(0, 0, ArrowCursorImage.width, ArrowCursorImage.height), Vector2.zero);

        trackerImg.sprite = trackerSprite;
        trackerImg.SetNativeSize();

        imageInstance = trackerImg;
        isVisible = true;
        return trackerImg;
    }

    public override void UpdateInstance()
    {

        var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.z < 0f)
        {
            screenPosition *= -1f;
            screenPosition = (screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f))*Utility.ProjectOffscreenLength + new Vector3(screenCentre.x, screenCentre.y, 0f);
        }
        screenPosition.z = 0f;

        if (screenBounds.Contains(screenPosition))
        {
            var distanceSquared = (transform.position - Universe.Current.ViewPort.transform.position).sqrMagnitude;
            var useSprite = trackerSprite;
            if (distanceSquared > 1000f*1000f)
            {
                useSprite = farTrackerSprite;
                if (distanceSquared > 2000f*2000f)
                {
                    useSprite = veryFarTrackerSprite;
                }
            }
            imageInstance.sprite = useSprite;
            imageInstance.rectTransform.localPosition = screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f);
            imageInstance.rectTransform.localRotation = Quaternion.identity;
        }
        else
        {
            imageInstance.sprite = arrowSprite;
            imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, screenBounds);
            imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));
        }
    }

    public void TriggerFadeIn()
    {
        if (!isFading && !isVisible)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void TriggerFadeOut()
    {
        if (!isFading && isVisible)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        for (var fraction = 1f; fraction >= 0f; fraction -= 0.1f)
        {
            imageInstance.color = new Color(1f, 1f, 1f, fraction);
            yield return new WaitForSeconds(0.05f);
        }
        imageInstance.color = new Color(1f, 1f, 1f, 0f);
        isVisible = false;
        isFading = false;
    }

    private IEnumerator FadeIn()
    {
        for (var fraction = 1f; fraction >= 0f; fraction -= 0.1f)
        {
            imageInstance.color = new Color(1f, 1f, 1f, 1f - fraction);
            yield return new WaitForSeconds(0.05f);
        }
        imageInstance.color = new Color(1f, 1f, 1f, 1f);
        isVisible = true;
        isFading = false;
    }

    public override void DestroyInstance()
    {
        if (imageInstance != null)
            Destroy(imageInstance);
    }

    public override void SetVisible(bool value)
    {
        imageInstance.enabled = value;
    }

    private float GetScreenAngle(Vector2 point)
    {
        var delta = point - screenCentre;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
