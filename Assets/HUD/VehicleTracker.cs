using UnityEngine;
using UnityEngine.UI;

public class VehicleTracker : Tracker
{
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCursorImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;
    public Texture2D LockingCursorImage;
    public Texture2D LockedCursorImage;
    public string CallSign;
    public bool IsDisabled;

    private Vector2 screenCentre;
    private Rect screenBounds;
    private Image imageInstance;
    private Image healthBarBackgroundInstance;
    private Image healthBarInstance;

    private Sprite trackerSprite;
    private Sprite farTrackerSprite;
    private Sprite veryFarTrackerSprite;
    private Sprite arrowSprite;
    private Sprite lockingSprite;
    private Sprite lockedSprite;

    private Texture2D healthBarTexture;
    private Texture2D healthBarBackgroundTexture;

    private Targetable targetable;
    private bool isLockedOn;

    private void Awake()
    {
        screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
        var boundaryPadding = 20f;
        screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);

        healthBarTexture = Utility.ColouredTexture(48, 2, new Color(1f, 1f, 1f, 1f));
        healthBarBackgroundTexture = Utility.ColouredTexture(48, 2, new Color(1f, 1f, 1f, 0.05f));

        targetable = GetComponent<Targetable>();
        isLockedOn = false;
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
        lockingSprite = Sprite.Create(LockingCursorImage, new Rect(0, 0, LockingCursorImage.width, LockingCursorImage.height), Vector2.zero);
        lockedSprite = Sprite.Create(LockedCursorImage, new Rect(0, 0, LockedCursorImage.width, LockedCursorImage.height), Vector2.zero);

        trackerImg.sprite = trackerSprite;
        trackerImg.SetNativeSize();

        // Healthbar background
        var healthBarBackgroundObj = new GameObject(string.Format("{0}_HealthBackground", transform.name));
        var healthBarBackgroundImg = healthBarBackgroundObj.AddComponent<Image>();
        healthBarBackgroundImg.rectTransform.pivot = new Vector2(0.5f, -15f);
        healthBarBackgroundImg.color = new Color(1f, 1f, 1f, 1f);
        healthBarBackgroundImg.sprite = Sprite.Create(healthBarBackgroundTexture, new Rect(0, 0, healthBarBackgroundTexture.width, healthBarBackgroundTexture.height), Vector2.zero);
        healthBarBackgroundImg.SetNativeSize();

        // Healthbar
        var healthBarObj = new GameObject(string.Format("{0}_HealthBackground", transform.name));
        var healthBarImg = healthBarObj.AddComponent<Image>();
        healthBarImg.rectTransform.pivot = new Vector2(0.5f, -15f);
        healthBarImg.color = new Color(1f, 1f, 1f, 1f);
        healthBarImg.sprite = Sprite.Create(healthBarTexture, new Rect(0, 0, healthBarTexture.width, healthBarTexture.height), Vector2.zero);

        healthBarImg.type = Image.Type.Filled;
        healthBarImg.fillMethod = Image.FillMethod.Horizontal;
        healthBarImg.fillAmount = 1f;

        healthBarImg.SetNativeSize();

        healthBarBackgroundObj.transform.SetParent(trackerImg.transform);
        healthBarObj.transform.SetParent(trackerImg.transform);

        healthBarBackgroundInstance = healthBarBackgroundImg;
        healthBarInstance = healthBarImg;

        imageInstance = trackerImg;
        return trackerImg;
    }

    public override void UpdateInstance()
    {
        if (IsDisabled)
        {
            imageInstance.enabled = false;
            healthBarBackgroundInstance.enabled = false;
            healthBarInstance.enabled = false;
        }
        else
        {
            imageInstance.enabled = true;
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
                    healthBarBackgroundInstance.enabled = false;
                    healthBarInstance.enabled = false;
                }
                else
                {
                    UpdateHealthBar();
                }
                // Locking
                isLockedOn = false;
                var playerVehicle = PlayerController.Current.VehicleInstance;
                if (playerVehicle != null)
                {
                    if (playerVehicle.PrimaryWeaponInstance != null)
                    {
                        if (playerVehicle.PrimaryWeaponInstance.GetLockingOnTarget() == transform)
                            useSprite = lockingSprite;
                        if (targetable != null && targetable.LockedOnBy == playerVehicle.transform || playerVehicle.PrimaryWeaponInstance.GetLockedOnTarget() == transform)
                            isLockedOn = true;
                        if (isLockedOn)
                            useSprite = lockedSprite;
                    }
                    if (playerVehicle.SecondaryWeaponInstance != null)
                    {
                        if (playerVehicle.SecondaryWeaponInstance.GetLockingOnTarget() == transform)
                            useSprite = lockingSprite;
                        if (targetable != null && targetable.LockedOnBy == playerVehicle.transform || playerVehicle.SecondaryWeaponInstance.GetLockedOnTarget() == transform)
                            isLockedOn = true;
                        if (isLockedOn)
                            useSprite = lockedSprite;
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

                healthBarBackgroundInstance.enabled = false;
                healthBarInstance.enabled = false;
            }
        }
    }

    private void UpdateHealthBar()
    {
        var ownerKillable = gameObject.GetComponent<Killable>();
        if (ownerKillable != null)
        {
            var healthFraction = Mathf.Clamp01(ownerKillable.Health / ownerKillable.MaxHealth);
            if (healthFraction < 1f)
            {
                healthBarInstance.fillAmount = healthFraction;
                healthBarBackgroundInstance.enabled = true;
                healthBarInstance.enabled = true;
            }
            else
            {
                healthBarBackgroundInstance.enabled = false;
                healthBarInstance.enabled = false;
            }
        }
    }

    public override void DestroyInstance()
    {
        if (imageInstance != null)
            Destroy(imageInstance.gameObject);
    }

    public override void SetVisible(bool value)
    {
        IsDisabled = !value;
        UpdateInstance();
    }

    private float GetScreenAngle(Vector2 point)
    {
        var delta = point - screenCentre;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
        return angle;
    }
}
