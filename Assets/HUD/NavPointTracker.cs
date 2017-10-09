using UnityEngine;
using UnityEngine.UI;

public class NavPointTracker : Tracker
{
    public Texture2D TrackerCursorImage;
    public Texture2D ArrowCursorImage;
    public Font LabelFont;

    private Vector2 _screenCentre;
    private Rect _screenBounds;
    private Image _imageInstance;
    private Text _labelInstance;

    private Sprite _trackerSprite;
    private Sprite _arrowSprite;

    private Shiftable _shiftable;

    private void Awake()
    {
        _screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
        var boundaryPadding = 20f;
        _screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);
        _shiftable = GetComponent<Shiftable>();
    }

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);

        _trackerSprite = Sprite.Create(TrackerCursorImage, new Rect(0, 0, TrackerCursorImage.width, TrackerCursorImage.height), Vector2.zero);
        _arrowSprite = Sprite.Create(ArrowCursorImage, new Rect(0, 0, ArrowCursorImage.width, ArrowCursorImage.height), Vector2.zero);

        trackerImg.sprite = _trackerSprite;
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

        _labelInstance = labelText;
        _imageInstance = trackerImg;
        return trackerImg;
    }

    public override void UpdateInstance()
    {
        var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.z < 0f)
        {
            screenPosition *= -1f;
            screenPosition = (screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f))*Utility.ProjectOffscreenLength + new Vector3(_screenCentre.x, _screenCentre.y, 0f);
        }
        screenPosition.z = 0f;

        if (_screenBounds.Contains(screenPosition))
        {
            _imageInstance.sprite = _trackerSprite;
            _imageInstance.rectTransform.localPosition = screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f);
            _imageInstance.rectTransform.localRotation = Quaternion.identity;

            if (Player.Current.VehicleInstance != null)
            {
                var fromPlayer = _shiftable.GetAbsoluteUniversePosition() - Player.Current.VehicleInstance.Shiftable.GetAbsoluteUniversePosition();
                _labelInstance.text = DistanceDisplay.GetDistanceString(fromPlayer.magnitude);
                _labelInstance.enabled = true;
            }
        }
        else
        {
            _imageInstance.sprite = _arrowSprite;
            _imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, _screenBounds);
            _imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));

            _labelInstance.enabled = false;
        }
    }

    public override void DestroyInstance()
    {
        if (_imageInstance != null)
            Destroy(_imageInstance);
    }

    public override void SetVisible(bool value)
    {
        _imageInstance.enabled = value;
        _labelInstance.enabled = value;
    }

    private float GetScreenAngle(Vector2 point)
    {
        var delta = point - _screenCentre;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
