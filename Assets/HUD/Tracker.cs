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

	void Start()
	{
		UniverseTrackers.Current.AddTracker(this);
	}

	void OnDestroy()
	{
		UniverseTrackers.Current.RemoveTracker(this);
	}
}
