using UnityEngine;
using UnityEngine.UI;

public class VehicleTracker : Tracker
{
	public Texture2D ArrowCursorImage;
	public Texture2D TrackerCursorImage;
	public Texture2D FarTrackerCursorImage;
	public Texture2D VeryFarTrackerCursorImage;
	public Color TrackerColor = Color.white;
	public Texture2D LockingCursorImage;
	public Texture2D LockedCursorImage;

    public Targetable Targetable;
    public Killable Killable;

    public float MaxDistance = 3000f;

	public bool IsDisabled;

	private Vector2 _screenCentre;
	private Rect _screenBounds;
	private Image _imageInstance;
    private Image _shieldBarBackgroundInstance;
    private Image _shieldBarInstance;
    private Image _healthBarBackgroundInstance;
	private Image _healthBarInstance;

	private Sprite _trackerSprite;
	private Sprite _farTrackerSprite;
	private Sprite _veryFarTrackerSprite;
	private Sprite _arrowSprite;
	private Sprite _lockingSprite;
	private Sprite _lockedSprite;

    private Texture2D _shieldBarTexture;
	private Texture2D _healthBarTexture;
	private Texture2D _healthBarBackgroundTexture;

	private Targetable _targetable;
    private Killable _killable;
    private float _maxDistanceSquared;
	private bool _isLockedOn;

	private void Awake()
	{
		_screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
		var boundaryPadding = 20f;
		_screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);

        _shieldBarTexture = Utility.ColouredTexture(48, 2, new Color(0.8f, 0.8f, 1f, 1f));
        _healthBarTexture = Utility.ColouredTexture(48, 2, new Color(1f, 1f, 1f, 1f));
		_healthBarBackgroundTexture = Utility.ColouredTexture(48, 2, new Color(1f, 1f, 1f, 0.05f));

	    _targetable = Targetable ?? GetComponent<Targetable>();
	    _killable = Killable ?? GetComponent<Killable>();
	    _maxDistanceSquared = MaxDistance*MaxDistance;

	    _isLockedOn = false;
	}

	public override Image CreateInstance()
	{
		var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
		var trackerImg = trackerObj.AddComponent<Image>();

		trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		trackerImg.color = new Color(1f, 1f, 1f, 1f);

		_trackerSprite = Sprite.Create(TrackerCursorImage, new Rect(0, 0, TrackerCursorImage.width, TrackerCursorImage.height), Vector2.zero);
		_farTrackerSprite = Sprite.Create(FarTrackerCursorImage, new Rect(0, 0, FarTrackerCursorImage.width, FarTrackerCursorImage.height), Vector2.zero);
		_veryFarTrackerSprite = Sprite.Create(VeryFarTrackerCursorImage, new Rect(0, 0, VeryFarTrackerCursorImage.width, VeryFarTrackerCursorImage.height), Vector2.zero);
		_arrowSprite = Sprite.Create(ArrowCursorImage, new Rect(0, 0, ArrowCursorImage.width, ArrowCursorImage.height), Vector2.zero);
		_lockingSprite = Sprite.Create(LockingCursorImage, new Rect(0, 0, LockingCursorImage.width, LockingCursorImage.height), Vector2.zero);
		_lockedSprite = Sprite.Create(LockedCursorImage, new Rect(0, 0, LockedCursorImage.width, LockedCursorImage.height), Vector2.zero);

		trackerImg.sprite = _trackerSprite;
		trackerImg.SetNativeSize();

        // Shieldbar background
        var shieldBarBackgroundObj = new GameObject(string.Format("{0}_ShieldBackground", transform.name));
        var shieldBarBackgroundImg = shieldBarBackgroundObj.AddComponent<Image>();
        shieldBarBackgroundImg.rectTransform.pivot = new Vector2(0.5f, -17f);
        shieldBarBackgroundImg.color = new Color(1f, 1f, 1f, 1f);
        shieldBarBackgroundImg.sprite = Sprite.Create(_healthBarBackgroundTexture, new Rect(0, 0, _healthBarBackgroundTexture.width, _healthBarBackgroundTexture.height), Vector2.zero);
        shieldBarBackgroundImg.SetNativeSize();

        // Shieldbar
        var shieldBarObj = new GameObject(string.Format("{0}_Shield", transform.name));
        var shieldBarImg = shieldBarObj.AddComponent<Image>();
        shieldBarImg.rectTransform.pivot = new Vector2(0.5f, -17f);
        shieldBarImg.color = new Color(1f, 1f, 1f, 1f);
        shieldBarImg.sprite = Sprite.Create(_shieldBarTexture, new Rect(0, 0, _shieldBarTexture.width, _shieldBarTexture.height), Vector2.zero);

        shieldBarImg.type = Image.Type.Filled;
        shieldBarImg.fillMethod = Image.FillMethod.Horizontal;
        shieldBarImg.fillAmount = 1f;

        shieldBarImg.SetNativeSize();

        // Healthbar background
        var healthBarBackgroundObj = new GameObject(string.Format("{0}_HealthBackground", transform.name));
		var healthBarBackgroundImg = healthBarBackgroundObj.AddComponent<Image>();
		healthBarBackgroundImg.rectTransform.pivot = new Vector2(0.5f, -15f);
		healthBarBackgroundImg.color = new Color(1f, 1f, 1f, 1f);
		healthBarBackgroundImg.sprite = Sprite.Create(_healthBarBackgroundTexture, new Rect(0, 0, _healthBarBackgroundTexture.width, _healthBarBackgroundTexture.height), Vector2.zero);
		healthBarBackgroundImg.SetNativeSize();

		// Healthbar
		var healthBarObj = new GameObject(string.Format("{0}_Health", transform.name));
		var healthBarImg = healthBarObj.AddComponent<Image>();
		healthBarImg.rectTransform.pivot = new Vector2(0.5f, -15f);
		healthBarImg.color = new Color(1f, 1f, 1f, 1f);
		healthBarImg.sprite = Sprite.Create(_healthBarTexture, new Rect(0, 0, _healthBarTexture.width, _healthBarTexture.height), Vector2.zero);

		healthBarImg.type = Image.Type.Filled;
		healthBarImg.fillMethod = Image.FillMethod.Horizontal;
		healthBarImg.fillAmount = 1f;

		healthBarImg.SetNativeSize();

        // Stuff
        shieldBarBackgroundObj.transform.SetParent(trackerImg.transform);
        shieldBarObj.transform.SetParent(trackerImg.transform);

	    _shieldBarBackgroundInstance = shieldBarBackgroundImg;
	    _shieldBarInstance = shieldBarImg;

        healthBarBackgroundObj.transform.SetParent(trackerImg.transform);
		healthBarObj.transform.SetParent(trackerImg.transform);

        _healthBarBackgroundInstance = healthBarBackgroundImg;
        _healthBarInstance = healthBarImg;

        _healthBarBackgroundInstance = healthBarBackgroundImg;
		_healthBarInstance = healthBarImg;

		_imageInstance = trackerImg;
		_imageInstance.color = TrackerColor;

        var distanceSquared = (_targetable.transform.position - Universe.Current.ViewPort.transform.position).sqrMagnitude;

        _imageInstance.color = Utility.SetColorAlpha(_imageInstance.color, 0f);
        _shieldBarBackgroundInstance.color = Utility.SetColorAlpha(_shieldBarBackgroundInstance.color, 0f);
        _shieldBarInstance.color = Utility.SetColorAlpha(_shieldBarInstance.color, 0f);
        _healthBarBackgroundInstance.color = Utility.SetColorAlpha(_healthBarBackgroundInstance.color, 0f);
        _healthBarInstance.color = Utility.SetColorAlpha(_healthBarInstance.color, 0f);

	    if (distanceSquared < _maxDistanceSquared)
	    {
	        fadeDirection = 1;
	        fadeCooldown = fadeTime;
	    }
	    else
	    {
	        fadeCooldown = -1f;
	    }
	    lastDistanceSquared = distanceSquared;

        return trackerImg;
	}

    private int fadeDirection;
    private float fadeCooldown;
    private float fadeTime = 1f;
    private float lastDistanceSquared;

    public override void UpdateInstance()
    {
        var distanceSquared = (_targetable.transform.position - Universe.Current.ViewPort.transform.position).sqrMagnitude;

        if (lastDistanceSquared < _maxDistanceSquared)
        {
            if (distanceSquared > _maxDistanceSquared)
            {
                // Trigger Fade Out!
                fadeDirection = -1;
                fadeCooldown = fadeTime;
            }
        }
        if (lastDistanceSquared > _maxDistanceSquared)
        {
            if (distanceSquared < _maxDistanceSquared)
            {
                // Trigger fade in!
                fadeDirection = 1;
                fadeCooldown = fadeTime;
            }
        }

        if (fadeCooldown >= 0f)
        {
            fadeCooldown -= Time.deltaTime;
            var fadeFraction = Mathf.Clamp01(fadeCooldown/fadeTime);
            if (fadeDirection > 0)
            {
                _imageInstance.color = Utility.SetColorAlpha(_imageInstance.color, 1f- fadeFraction);
                _shieldBarBackgroundInstance.color = Utility.SetColorAlpha(_shieldBarBackgroundInstance.color, 1f - fadeFraction);
                _shieldBarInstance.color = Utility.SetColorAlpha(_shieldBarInstance.color, 1f - fadeFraction);
                _healthBarBackgroundInstance.color = Utility.SetColorAlpha(_healthBarBackgroundInstance.color, 1f- fadeFraction);
                _healthBarInstance.color = Utility.SetColorAlpha(_healthBarInstance.color, 1f- fadeFraction);
            }
            else
            {
                _imageInstance.color = Utility.SetColorAlpha(_imageInstance.color, fadeFraction);
                _shieldBarBackgroundInstance.color = Utility.SetColorAlpha(_shieldBarBackgroundInstance.color, fadeFraction);
                _shieldBarInstance.color = Utility.SetColorAlpha(_shieldBarInstance.color, fadeFraction);
                _healthBarBackgroundInstance.color = Utility.SetColorAlpha(_healthBarBackgroundInstance.color, fadeFraction);
                _healthBarInstance.color = Utility.SetColorAlpha(_healthBarInstance.color, fadeFraction);
            }
        }

        if (IsDisabled)
        {
            _imageInstance.enabled = false;
            _shieldBarBackgroundInstance.enabled = false;
            _shieldBarInstance.enabled = false;
            _healthBarBackgroundInstance.enabled = false;
            _healthBarInstance.enabled = false;
        }
        else
        {
            _imageInstance.enabled = true;
            var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
            if (screenPosition.z < 0f)
            {
                screenPosition *= -1f;
                screenPosition = (screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f))*Utility.ProjectOffscreenLength + new Vector3(_screenCentre.x, _screenCentre.y, 0f);
            }
            screenPosition.z = 0f;

            if (_screenBounds.Contains(screenPosition))
            {

                var useSprite = _trackerSprite;
                if (distanceSquared > 1000f*1000f)
                {
                    useSprite = _farTrackerSprite;
                    if (distanceSquared > 2000f*2000f)
                    {
                        useSprite = _veryFarTrackerSprite;
                    }
                    _shieldBarBackgroundInstance.enabled = false;
                    _shieldBarInstance.enabled = false;
                    _healthBarBackgroundInstance.enabled = false;
                    _healthBarInstance.enabled = false;
                }
                else
                {
                    UpdateHealthBar();
                }
                // Locking
                _isLockedOn = false;
                var playerVehicle = PlayerController.Current.VehicleInstance;
                if (playerVehicle != null)
                {
                    if (playerVehicle.PrimaryWeaponInstance != null)
                    {
                        if (playerVehicle.PrimaryWeaponInstance.GetLockingOnTarget() == _targetable.transform)
                            useSprite = _lockingSprite;
                        if (_targetable != null && _targetable.LockedOnBy == playerVehicle.transform || playerVehicle.PrimaryWeaponInstance.GetLockedOnTarget() == _targetable.transform)
                            _isLockedOn = true;
                        if (_isLockedOn)
                            useSprite = _lockedSprite;
                    }
                    if (playerVehicle.SecondaryWeaponInstance != null)
                    {
                        if (playerVehicle.SecondaryWeaponInstance.GetLockingOnTarget() == _targetable.transform)
                            useSprite = _lockingSprite;
                        if (_targetable != null && _targetable.LockedOnBy == playerVehicle.transform || playerVehicle.SecondaryWeaponInstance.GetLockedOnTarget() == _targetable.transform)
                            _isLockedOn = true;
                        if (_isLockedOn)
                            useSprite = _lockedSprite;
                    }
                }
                // Dodgey method of colouring locking cursors white.
                if (useSprite == _lockingSprite || useSprite == _lockedSprite)
                {
                    _imageInstance.color = Color.white;
                }
                else
                {
                    _imageInstance.color = Utility.SetColorAlpha(TrackerColor, _imageInstance.color.a);
                }
                _imageInstance.sprite = useSprite;

                _imageInstance.rectTransform.localPosition = screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f);
                _imageInstance.rectTransform.localRotation = Quaternion.identity;
            }
            else
            {
                _imageInstance.sprite = _arrowSprite;
                _imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, _screenBounds);
                _imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));

                _shieldBarBackgroundInstance.enabled = false;
                _shieldBarInstance.enabled = false;
                _healthBarBackgroundInstance.enabled = false;
                _healthBarInstance.enabled = false;
            }
        }

        lastDistanceSquared = distanceSquared;
    }

    private void UpdateHealthBar()
    {
        if (_killable != null)
        {
            var shieldFraction = Mathf.Clamp01(_killable.Shield/_killable.MaxShield);
            var healthFraction = Mathf.Clamp01(_killable.Health/_killable.MaxHealth);

            if (shieldFraction < 1f || healthFraction < 1f)
            {
                if (_killable.MaxHealth > 0f)
                {
                    _shieldBarInstance.fillAmount = shieldFraction;
                    _shieldBarBackgroundInstance.enabled = true;
                    _shieldBarInstance.enabled = true;
                }
                else
                {
                    _shieldBarBackgroundInstance.enabled = false;
                    _shieldBarInstance.enabled = false;
                }

                _healthBarInstance.fillAmount = healthFraction;
                _healthBarBackgroundInstance.enabled = true;
                _healthBarInstance.enabled = true;
            }
            else
            {
                _shieldBarBackgroundInstance.enabled = false;
                _shieldBarInstance.enabled = false;

                _healthBarBackgroundInstance.enabled = false;
                _healthBarInstance.enabled = false;
            }
        }
    }

    public override void DestroyInstance()
	{
		if (_imageInstance != null)
			Destroy(_imageInstance.gameObject);
	}

	public override void SetVisible(bool value)
	{
		IsDisabled = !value;
		UpdateInstance();
	}

	private float GetScreenAngle(Vector2 point)
	{
		var delta = point - _screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
