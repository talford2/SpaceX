using UnityEngine;
using UnityEngine.UI;

public class NavPointTracker : Tracker
{
    public Texture2D TrackerCursorImage;
    public Texture2D ArrowCursorImage;
    public Font LabelFont;

    private Vector2 screenCentre;
    private Rect screenBounds;
    private Image imageInstance;
    private Text labelInstance;

    private Sprite trackerSprite;
    private Sprite arrowSprite;

    private Shiftable shiftable;

    private void Awake()
    {
        screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
        var boundaryPadding = 20f;
        screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);
        shiftable = GetComponent<Shiftable>();
    }

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);

        trackerSprite = Sprite.Create(TrackerCursorImage, new Rect(0, 0, TrackerCursorImage.width, TrackerCursorImage.height), Vector2.zero);
        arrowSprite = Sprite.Create(ArrowCursorImage, new Rect(0, 0, ArrowCursorImage.width, ArrowCursorImage.height), Vector2.zero);

        trackerImg.sprite = trackerSprite;
        trackerImg.SetNativeSize();

        var labelObject = new GameObject(string.Format("{0}_Label", transform.name));
        var labelText = labelObject.AddComponent<Text>();

        labelText.font = LabelFont;
        labelText.fontSize = 10;
        labelText.rectTransform.pivot = new Vector2(0.5f, 0.9f);
        labelText.color = new Color(1f, 1f, 1f, 1f);
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.rectTransform.localScale = Vector3.one;

        labelObject.transform.SetParent(trackerObj.transform);

        labelInstance = labelText;
        imageInstance = trackerImg;
        return trackerImg;
    }

    public override void UpdateInstance()
    {
        var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.z < 0f)
        {
            screenPosition *= -1f;
            screenPosition = (screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f))*20000f + new Vector3(screenCentre.x, screenCentre.y, 0f);
        }
        screenPosition.z = 0f;

        if (screenBounds.Contains(screenPosition))
        {
            imageInstance.sprite = trackerSprite;
            imageInstance.rectTransform.localPosition = screenPosition - new Vector3(screenCentre.x, screenCentre.y, 0f);
            imageInstance.rectTransform.localRotation = Quaternion.identity;

            if (PlayerController.Current.VehicleInstance != null)
            {
                var fromPlayer = shiftable.GetAbsoluteUniversePosition() - PlayerController.Current.VehicleInstance.Shiftable.GetAbsoluteUniversePosition();
                labelInstance.text = DistanceDisplay.GetDistanceString(fromPlayer.magnitude);
                labelInstance.enabled = true;
            }
        }
        else
        {
            imageInstance.sprite = arrowSprite;
            imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, screenBounds);
            imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));

            labelInstance.enabled = false;
        }
    }

    public override void DestroyInstance()
    {
        if (imageInstance != null)
            Destroy(imageInstance);
    }

    private float GetScreenAngle(Vector2 point)
    {
        var delta = point - screenCentre;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
