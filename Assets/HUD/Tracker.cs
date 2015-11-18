using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tracker : MonoBehaviour
{
	public Texture2D ArrowCursorImage;
	public Texture2D TrackerCurosrImage;

	public Image ArrowCursor { get; set; }
	public Image TrackerCurosr { get; set; }

	void Start()
	{
		UniverseTrackers.AddTracker(this);
	}

	void OnDestroy()
	{
		UniverseTrackers.RemoveTracker(this);
	}
}
