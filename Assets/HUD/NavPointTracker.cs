using UnityEngine;
using UnityEngine.UI;

public class NavPointTracker : Tracker
{
    public Texture2D TrackerCursorImage;

    private Vector2 screenCentre;
    private Rect screenBounds;
    private Image imageInstance;

    private void Awake()
    {
        screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
        var boundaryPadding = 20f;
        screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);
    }

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);
        trackerImg.sprite = Sprite.Create(TrackerCursorImage, new Rect(0, 0, TrackerCursorImage.width, TrackerCursorImage.height), Vector2.zero);
        trackerImg.SetNativeSize();

        imageInstance = trackerImg;
        return trackerImg;
    }

    public override void UpdateInstance()
    {
        var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
        imageInstance.rectTransform.localPosition = screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f);
        if (screenBounds.Contains(screenPosition))
        {
            imageInstance.rectTransform.localPosition = screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f);
            imageInstance.rectTransform.localRotation = Quaternion.identity;
        }
        else
        {
            imageInstance.rectTransform.localPosition = GetBoundsIntersection(screenPosition, screenBounds);
            imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));
        }
    }

    private float GetScreenAngle(Vector2 point)
    {
        var delta = point - screenCentre;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
