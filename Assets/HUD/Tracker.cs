using UnityEngine;
using UnityEngine.UI;

public class Tracker : MonoBehaviour
{
    public Texture2D ArrowCursorImage;
    public Texture2D TrackerCurosrImage;
    public Texture2D FarTrackerCursorImage;
    public Texture2D VeryFarTrackerCursorImage;
    public bool IsDisabled;

    public Image ArrowCursor { get; set; }
    public Image TrackerCurosr { get; set; }
    public Image FarTrackerCursor { get; set; }
    public Image VeryFarTrackerCursor { get; set; }
    public Image HealthBarBackground { get; set; }
    public Image HealthBar { get; set; }

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
            if (distanceSquared > 2000f*2000f)
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
        TrackerCurosr.enabled = true;
        FarTrackerCursor.enabled = false;
        VeryFarTrackerCursor.enabled = false;
        return TrackerCurosr;
    }
}
