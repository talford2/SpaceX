using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VehicleTracker : Tracker
{
    //public Sprite ArrowSprite;
    //   //public Sprite TrackerSprite;
    //   //public Sprite FarTrackerSprite;
    //   //public Sprite VeryFarTrackerSprite;
    //   public Color TrackerColor = Color.white;
    //public Sprite LockingSprite;
    //public Sprite LockedSprite;

    //   public Color BackgroundColor = new Color(0f, 0f, 0f, 0.5f);
    //   public Color ShieldBarColor = new Color(0.6f, 0.6f, 1f, 1f);
    //   public Color HealthBarColor = new Color(1f, 1f, 1f, 1f);

    //   public GameObject TrackerPlanePrefab;
    //   public float TrackerPlaneScale = 100f;
    public VehicleTrackerValues Options;

    public Targetable Targetable;
    public Killable Killable;

    //   public float MaxDistance = 3000f;

    public bool IsDisabled;

    private Vector2 _screenCentre;
    private Rect _screenBounds;
    private Image _imageInstance;
    private Image _shieldBarBackgroundInstance;
    private Image _shieldBarInstance;
    private Image _healthBarBackgroundInstance;
    private Image _healthBarInstance;
    private Image _lockInstance;

    //private Sprite _trackerSprite;
    //private Sprite _farTrackerSprite;
    //private Sprite _veryFarTrackerSprite;
    private Sprite _arrowSprite;
    private Sprite _lockingSprite;
    private Sprite _lockedSprite;

    private GameObject _trackerPlaneInstance;
    private MeshRenderer _trackerPlaneRenderer;

    private Texture2D _shieldBarTexture;
    private Texture2D _healthBarTexture;
    private Texture2D _healthBarBackgroundTexture;

    private Targetable _targetable;
    private Killable _killable;
    private bool _isLockedOn;
    private bool _oldLockedOn;

    protected bool InScreenBounds;

    private bool _isVisible;
    private bool _isFading;

    private float _fadeStep = 0.02f;

    private Shiftable _shiftable;
    private float _maxDistanceSquared;
    private float _lastDistanceSquared;

    private float _lockingCooldown;
    private float _lockingDuration;

    private void Awake()
    {
        _shiftable = GetComponentInParent<Shiftable>();

        _screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
        var boundaryPadding = 20f;
        _screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);

        _targetable = Targetable ?? GetComponent<Targetable>();
        _killable = Killable ?? GetComponent<Killable>();

        _isLockedOn = false;
    }

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        _shieldBarTexture = Utility.ColouredTexture(48, 2, Options.ShieldBarColor);
        _healthBarTexture = Utility.ColouredTexture(48, 2, Options.HealthBarColor);
        _healthBarBackgroundTexture = Utility.ColouredTexture(48, 2, Options.BackgroundColor);
        _maxDistanceSquared = Options.MaxDistance * Options.MaxDistance;

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);

        //_trackerSprite = TrackerSprite;
        //_farTrackerSprite = FarTrackerSprite;
        //_veryFarTrackerSprite = VeryFarTrackerSprite;
        _arrowSprite = Options.ArrowSprite;

        _lockingSprite = Options.LockingSprite;
        _lockedSprite = Options.LockedSprite;

        trackerImg.sprite = null; // _trackerSprite;
        trackerImg.SetNativeSize();

        // Locking tracker
        var lockObj = new GameObject(string.Format("{0}_LockTracker", transform.name));
        var lockImg = lockObj.AddComponent<Image>();

        lockImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        lockImg.color = new Color(1f, 1f, 1f, 1f);
        lockImg.sprite = _lockingSprite;
        lockImg.SetNativeSize();

        // Shieldbar background
        var shieldBarBackgroundObj = new GameObject(string.Format("{0}_ShieldBackground", transform.name));
        var shieldBarBackgroundImg = shieldBarBackgroundObj.AddComponent<Image>();
        shieldBarBackgroundImg.rectTransform.pivot = new Vector2(0.5f, -16.5f);
        shieldBarBackgroundImg.color = new Color(1f, 1f, 1f, 1f);
        shieldBarBackgroundImg.sprite = Sprite.Create(_healthBarBackgroundTexture, new Rect(0, 0, _healthBarBackgroundTexture.width, _healthBarBackgroundTexture.height), Vector2.zero);
        shieldBarBackgroundImg.SetNativeSize();

        // Shieldbar
        var shieldBarObj = new GameObject(string.Format("{0}_Shield", transform.name));
        var shieldBarImg = shieldBarObj.AddComponent<Image>();
        shieldBarImg.rectTransform.pivot = new Vector2(0.5f, -16.5f);
        shieldBarImg.color = new Color(1f, 1f, 1f, 1f);
        shieldBarImg.sprite = Sprite.Create(_shieldBarTexture, new Rect(0, 0, _shieldBarTexture.width, _shieldBarTexture.height), Vector2.zero);

        shieldBarImg.type = Image.Type.Filled;
        shieldBarImg.fillMethod = Image.FillMethod.Horizontal;
        shieldBarImg.fillAmount = 1f;

        shieldBarImg.SetNativeSize();

        if (Options.TrackerPlanePrefab != null)
        {
            _trackerPlaneInstance = Instantiate(Options.TrackerPlanePrefab);
            _trackerPlaneRenderer = _trackerPlaneInstance.GetComponent<MeshRenderer>();
        }

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
        _imageInstance.color = Options.TrackerColor;

        _lockInstance = lockImg;
        _lockInstance.transform.SetParent(trackerImg.transform);
        _lockInstance.enabled = false;

        var distanceSquared = Universe.Current.SquareDistanceFromViewPort(_targetable.transform.position);

        _imageInstance.color = Utility.SetColorAlpha(_imageInstance.color, 0f);
        _shieldBarBackgroundInstance.color = Utility.SetColorAlpha(_shieldBarBackgroundInstance.color, 0f);
        _shieldBarInstance.color = Utility.SetColorAlpha(_shieldBarInstance.color, 0f);
        _healthBarBackgroundInstance.color = Utility.SetColorAlpha(_healthBarBackgroundInstance.color, 0f);
        _healthBarInstance.color = Utility.SetColorAlpha(_healthBarInstance.color, 0f);
        if (_trackerPlaneInstance != null)
            _trackerPlaneRenderer.material.color = Utility.SetColorAlpha(_trackerPlaneRenderer.material.color, 0f);

        _isVisible = false;
        SetAlpha(0f);
        _lastDistanceSquared = Mathf.Infinity;

        return trackerImg;
    }

    public override void UpdateInstance()
    {
        if (isActiveAndEnabled)
        {
            var distanceSquared = Universe.Current.SquareDistanceFromViewPort(_targetable.transform.position);
            if (_lastDistanceSquared < _maxDistanceSquared)
            {
                if (distanceSquared > _maxDistanceSquared)
                {
                    TriggerFadeOut();
                }
            }

            if (_lastDistanceSquared > _maxDistanceSquared)
            {
                if (distanceSquared < _maxDistanceSquared)
                {
                    TriggerFadeIn();
                }
            }
            if (_trackerPlaneInstance != null)
            {
                var cameraPlane = new Plane(Universe.Current.ViewPort.transform.forward, Universe.Current.ViewPort.transform.position + 5f * Universe.Current.ViewPort.transform.forward);
                float planeDist;
                var toCamRay = new Ray(_targetable.transform.position, Universe.Current.ViewPort.transform.position - _targetable.transform.position);
                var dotCam = Vector3.Dot(_targetable.transform.position - Universe.Current.ViewPort.transform.position, Universe.Current.ViewPort.transform.forward);
                if (dotCam > 0f)
                {
                    cameraPlane.Raycast(toCamRay, out planeDist);
                    _trackerPlaneInstance.transform.position = toCamRay.GetPoint(planeDist);

                    _trackerPlaneInstance.transform.localRotation = Quaternion.LookRotation(-Universe.Current.ViewPort.transform.forward, Universe.Current.ViewPort.transform.up) * Quaternion.Euler(0, 0, 45f);

                    _trackerPlaneRenderer.enabled = true;

                    var dist = (Universe.Current.ViewPort.transform.position - _targetable.transform.position).magnitude;
                    var frac = Options.TrackerPlaneScale / dist;
                    _trackerPlaneRenderer.material.SetFloat("_Expand", Mathf.Clamp(frac, 0.18f, 1f));
                }
                else
                {
                    _trackerPlaneRenderer.enabled = false;
                }
            }

            InScreenBounds = false;
            if (IsDisabled)
            {
                _imageInstance.enabled = false;
                _shieldBarBackgroundInstance.enabled = false;
                _shieldBarInstance.enabled = false;
                _healthBarBackgroundInstance.enabled = false;
                _healthBarInstance.enabled = false;
                if (_trackerPlaneInstance != null)
                    _trackerPlaneRenderer.enabled = false;
            }
            else
            {
                if (_trackerPlaneInstance != null)
                    _trackerPlaneRenderer.enabled = true;
                _imageInstance.enabled = true;
                var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
                if (screenPosition.z < 0f)
                {
                    screenPosition *= -1f;
                    screenPosition = (screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f)) * Utility.ProjectOffscreenLength + new Vector3(_screenCentre.x, _screenCentre.y, 0f);
                }
                screenPosition.z = 0f;
                InScreenBounds = _screenBounds.Contains(screenPosition);
                if (InScreenBounds)
                {
                    //Sprite useSprite = null;// _trackerSprite;
                    if (distanceSquared > 1000f * 1000f)
                    {
                        /*
                        useSprite = _farTrackerSprite;
                        if (distanceSquared > 2000f * 2000f)
                        {
                            useSprite = _veryFarTrackerSprite;
                        }
                        */
                        _shieldBarBackgroundInstance.enabled = false;
                        _shieldBarInstance.enabled = false;
                        _healthBarBackgroundInstance.enabled = false;
                        _healthBarInstance.enabled = false;

                        _imageInstance.rectTransform.localRotation = Quaternion.identity;
                        _imageInstance.rectTransform.sizeDelta = 64f * Vector2.one;
                    }
                    else
                    {
                        UpdateHealthBar();

                        //_imageInstance.rectTransform.localRotation = Quaternion.Euler(0, 0, 45f);
                        //_imageInstance.rectTransform.sizeDelta = Mathf.Clamp(100f / Mathf.Sqrt(distanceSquared), 1f, 10f) * 64f * Vector2.one;
                        //_imageInstance.color = new Color(0f, 0f, 0f, 0f);
                    }

                    // Locking
                    _isLockedOn = false;
                    var playerVehicle = Player.Current.VehicleInstance;
                    if (playerVehicle != null)
                    {
                        if (playerVehicle.PrimaryWeaponInstance != null)
                        {
                            if (IsPlayerWeaponLocking(playerVehicle.PrimaryWeaponInstance))
                            {
                                _lockInstance.sprite = _lockingSprite;
                                _lockingDuration = playerVehicle.PrimaryWeaponInstance.Definition.TargetLockTime;
                                _isLockedOn = true;
                            }
                            if (IsPlayerWeaponLocked(playerVehicle, playerVehicle.PrimaryWeaponInstance))
                            {
                                _lockInstance.sprite = _lockedSprite;
                                _isLockedOn = true;
                            }
                        }
                        if (playerVehicle.SecondaryWeaponInstance != null)
                        {
                            if (IsPlayerWeaponLocking(playerVehicle.SecondaryWeaponInstance))
                            {
                                _lockInstance.sprite = _lockingSprite;
                                _lockingDuration = playerVehicle.SecondaryWeaponInstance.Definition.TargetLockTime;
                                _isLockedOn = true;
                            }
                            if (IsPlayerWeaponLocked(playerVehicle, playerVehicle.SecondaryWeaponInstance))
                            {
                                _lockInstance.sprite = _lockedSprite;
                                _isLockedOn = true;
                            }
                        }
                    }

                    if (_oldLockedOn != _isLockedOn)
                    {
                        _lockingCooldown = _lockingDuration;
                        _lockInstance.rectTransform.localScale = Vector3.one * 3f;
                        _lockInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                    }

                    var scale = Vector3.one;
                    var rotation = Quaternion.identity;
                    if (_isLockedOn)
                    {
                        _imageInstance.enabled = false;
                        _lockInstance.enabled = true;
                        if (_lockingCooldown >= 0f)
                            _lockingCooldown -= Time.deltaTime;
                        var lockingFraction = 1f - Mathf.Clamp01(_lockingCooldown / _lockingDuration);
                        rotation = Quaternion.AngleAxis(450f * lockingFraction, Vector3.forward);
                        scale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, lockingFraction);
                    }
                    else
                    {
                        _imageInstance.enabled = true;
                        _lockInstance.enabled = false;
                    }

                    //_imageInstance.sprite = useSprite;
                    _imageInstance.enabled = false;

                    _imageInstance.rectTransform.localPosition = screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f);
                    _imageInstance.rectTransform.localRotation = Quaternion.identity;

                    if (_lockInstance.enabled)
                    {
                        _lockInstance.rectTransform.localScale = scale;
                        _lockInstance.rectTransform.localRotation = rotation;
                    }
                }
                else
                {
                    _lockInstance.enabled = false;

                    if (Options.DisplyOffscreenArrow)
                    {
                        _imageInstance.enabled = true;
                        _imageInstance.sprite = _arrowSprite;
                        _imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, _screenBounds);
                        _imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));
                        _imageInstance.rectTransform.sizeDelta = 64f * Vector2.one;
                    }
                    else
                    {
                        _imageInstance.enabled = false;
                    }

                    _shieldBarBackgroundInstance.enabled = false;
                    _shieldBarInstance.enabled = false;
                    _healthBarBackgroundInstance.enabled = false;
                    _healthBarInstance.enabled = false;
                }
                _oldLockedOn = _isLockedOn;
            }
            _lastDistanceSquared = distanceSquared;
        }
    }

    public void TriggerFadeIn(float time = 0.5f)
    {
        if (!_isFading && !_isVisible)
        {
            StartCoroutine(FadeIn(time));
        }
    }

    public void TriggerFadeOut(float time = 0.5f)
    {
        if (!_isFading && _isVisible)
        {
            StartCoroutine(FadeOut(time));
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        var start = DateTime.Now;
        var ii = 0;

        var stepFraction = _fadeStep / duration;
        for (var fraction = 1f; fraction >= 0f; fraction -= stepFraction)
        {
            //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, fraction);
            SetAlpha(fraction);

            ii++;
            //Debug.Log("Time running: " + DateTime.Now.Subtract(start).TotalSeconds + " (" + ii + ")");

            yield return new WaitForSeconds(_fadeStep);
        }
        //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 0f);
        SetAlpha(0f);
        _isVisible = false;
        _isFading = false;
    }

    private IEnumerator FadeIn(float duration)
    {
        var stepFraction = _fadeStep / duration;
        for (var fraction = 1f; fraction >= 0f; fraction -= stepFraction)
        {
            //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 1f - fraction);
            SetAlpha(1f - fraction);
            yield return new WaitForSeconds(_fadeStep);
        }
        //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 1f);
        SetAlpha(1f);
        _isVisible = true;
        _isFading = false;
    }

    private void SetAlpha(float alpha)
    {
        if (_imageInstance != null)
            _imageInstance.color = Utility.SetColorAlpha(_imageInstance.color, alpha);
        if (_shieldBarBackgroundInstance != null)
            _shieldBarBackgroundInstance.color = Utility.SetColorAlpha(_shieldBarBackgroundInstance.color, alpha);
        if (_shieldBarInstance != null)
            _shieldBarInstance.color = Utility.SetColorAlpha(_shieldBarInstance.color, alpha);
        if (_healthBarBackgroundInstance != null)
            _healthBarBackgroundInstance.color = Utility.SetColorAlpha(_healthBarBackgroundInstance.color, alpha);
        if (_healthBarInstance != null)
            _healthBarInstance.color = Utility.SetColorAlpha(_healthBarInstance.color, alpha);
        if (_trackerPlaneInstance != null)
            _trackerPlaneRenderer.material.color = Utility.SetColorAlpha(_trackerPlaneRenderer.material.color, alpha);
    }

    private bool IsPlayerWeaponLocking(Weapon weapon)
    {
        return weapon.GetLockingOnTarget() == _targetable.transform;
    }

    private bool IsPlayerWeaponLocked(Vehicle playerVehicle, Weapon weapon)
    {
        return _targetable != null && _targetable.LockedOnBy == playerVehicle.transform || weapon.GetLockedOnTarget() == _targetable.transform;
    }

    private void UpdateHealthBar()
    {
        if (_killable != null)
        {
            var shieldFraction = Mathf.Clamp01(_killable.Shield / _killable.MaxShield);
            var healthFraction = Mathf.Clamp01(_killable.Health / _killable.MaxHealth);

            var isShowShieldBar = false;
            if (_killable.MaxShield > 0f)
            {
                if (shieldFraction < 1f || healthFraction < 1f)
                {
                    _shieldBarInstance.fillAmount = shieldFraction;
                    isShowShieldBar = true;
                }
            }
            _shieldBarBackgroundInstance.enabled = isShowShieldBar;
            _shieldBarInstance.enabled = isShowShieldBar;

            var isShowHealthBar = false;
            if (_killable.MaxHealth > 0f)
            {
                if (shieldFraction < 1f || healthFraction < 1f)
                {
                    _healthBarInstance.fillAmount = healthFraction;
                    isShowHealthBar = true;
                }
            }
            _healthBarBackgroundInstance.enabled = isShowHealthBar;
            _healthBarInstance.enabled = isShowHealthBar;
        }
    }

    public override void DestroyInstance()
    {
        if (_trackerPlaneInstance != null)
        {
            Destroy(_trackerPlaneInstance);
            Destroy(_trackerPlaneRenderer);
        }
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
