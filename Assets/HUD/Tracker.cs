using UnityEngine;
using UnityEngine.UI;

public class Tracker : MonoBehaviour
{
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCurosrImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;
    public Texture2D LockingCursorImage;
    public Texture2D LockedCursorImage;
    public string CallSign;
    public bool IsDisabled;

    public bool IsDelayComplete;
    public float DelayCooldown;

    public Image ArrowCursor { get; set; }
    public Image TrackerCurosr { get; set; }
    public Image FarTrackerCursor { get; set; }
    public Image VeryFarTrackerCursor { get; set; }
    public Image HealthBarBackground { get; set; }
    public Image HealthBar { get; set; }
    public Image LockingCursor { get; set; }
    public Image LockedCursor { get; set; }
    public Text CallSignText { get; set; }

    private void Start()
    {
        UniverseTrackers.Current.AddTracker(this);
    }

    private void OnDestroy()
    {
        UniverseTrackers.Current.RemoveTracker(this);
    }

    public void HideAllCursors()
    {
        ArrowCursor.enabled = false;
        TrackerCurosr.enabled = false;
        FarTrackerCursor.enabled = false;
        VeryFarTrackerCursor.enabled = false;
        HealthBarBackground.enabled = false;
        HealthBar.enabled = false;
        LockingCursor.enabled = false;
        LockedCursor.enabled = false;
        CallSignText.enabled = false;
    }

    public Image SwitchToArrow()
    {
        HideAllCursors();
        ArrowCursor.enabled = true;
        return ArrowCursor;
    }

    public Image SwitchCursor(float distanceSquared)
    {
        if (distanceSquared > 1000f*1000f)
        {
            HealthBarBackground.enabled = false;
            HealthBar.enabled = false;
            CallSignText.enabled = false;
            if (distanceSquared > 2000f * 2000f)
            {
                TrackerCurosr.enabled = false;
                FarTrackerCursor.enabled = false;
                VeryFarTrackerCursor.enabled = true;
                return VeryFarTrackerCursor;
            }
            TrackerCurosr.enabled = false;
            FarTrackerCursor.enabled = true;
            VeryFarTrackerCursor.enabled = false;
            return FarTrackerCursor;
        }
        LockingCursor.enabled = false;
        LockedCursor.enabled = false;
        if (PlayerController.Current.VehicleInstance != null)
        {
            if (PlayerController.Current.VehicleInstance.SecondaryWeaponInstance.GetLockedOnTarget() == transform)
            {
                LockedCursor.enabled = true;
            }
            else
            {
                if (PlayerController.Current.VehicleInstance.SecondaryWeaponInstance.GetLockingOnTarget() == transform)
                {
                    LockingCursor.enabled = true;
                }
            }
        }
        HealthBarBackground.enabled = true;
        HealthBar.enabled = true;
        TrackerCurosr.enabled = true;
        FarTrackerCursor.enabled = false;
        VeryFarTrackerCursor.enabled = false;
        CallSignText.text = CallSign;
        CallSignText.enabled = true;
        return TrackerCurosr;
    }
}
