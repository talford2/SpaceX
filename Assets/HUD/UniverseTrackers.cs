using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniverseTrackers : MonoBehaviour
{
	private Camera _cam;

	private Vector2 screenCentre;
	private float boundaryPadding = 20f;
	private Rect screenBounds;

	private Image cursor;

    private Texture2D healthBarBackgroundTexture;
    private Texture2D healthBarTexture;

	private static List<Tracker> _trackers { get; set; }

	public static UniverseTrackers Current;

	private void Awake()
	{
		Current = this;
	    healthBarBackgroundTexture = Utility.ColouredTexture(48, 2, new Color(1f, 1f, 1f, 0.3f));
	    healthBarTexture = Utility.ColouredTexture(48, 2, Color.white);
	}

	private void Start()
	{
		screenCentre = new Vector2(Screen.width / 2f, Screen.height / 2f);
		screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);

		var viewport = Universe.Current.ViewPort;
		_cam = viewport.GetComponent<Camera>();
		viewport.OnMove += ViewPortMove;
	}

	private void ViewPortMove()
	{
	    foreach (var tracker in _trackers)
	    {
	        if (!tracker.IsDisabled)
	        {
	            var screenPoint = _cam.WorldToScreenPoint(tracker.transform.position);
	            var r = screenPoint;
	            if (r.z < 0f)
	                r *= -1f;
	            r.z = 0f;
	            var inBounds = screenBounds.Contains(r);

	            if (inBounds)
	            {
	                
	                tracker.ArrowCursor.enabled = false;
                    tracker.HealthBarBackground.enabled = true;
                    tracker.HealthBar.enabled = true;

	                var toCamera = tracker.gameObject.transform.position - Universe.Current.ViewPort.transform.position;
	                cursor = tracker.SwitchCursor(toCamera.sqrMagnitude);
	            }
	            else
	            {
	                cursor = tracker.SwitchToArrow();
	            }

	            var dotThing = Vector3.Dot(tracker.transform.position - Universe.Current.ViewPort.transform.position, Universe.Current.ViewPort.transform.forward);
	            if (dotThing < 0f)
	            {
	                if (inBounds)
	                {
	                    //Debug.Log("BROKEN TRACKER!!!");
	                    //Debug.Log("SCREEN POINT: " + screenPoint);
	                    //Debug.Log("r: " + r);
	                    //Debug.Break();
	                }
	            }

	            cursor.rectTransform.localPosition = Utility.GetBoundsIntersection(new Vector2(r.x, r.y), screenBounds);
                tracker.HealthBar.rectTransform.localPosition = cursor.rectTransform.localPosition;
                tracker.HealthBarBackground.rectTransform.localPosition = cursor.rectTransform.localPosition;

	            var ownerKillable = tracker.gameObject.GetComponent<Killable>();
	            if (ownerKillable != null)
	            {
                    var healthFraction = Mathf.Clamp01(ownerKillable.Health / ownerKillable.MaxHealth);
	                if (healthFraction < 1f)
	                {
	                    tracker.HealthBar.fillAmount = healthFraction;
	                }
	                else
	                {
	                    tracker.HealthBar.enabled = false;
	                    tracker.HealthBarBackground.enabled = false;
	                }
	            }

	            if (!inBounds)
	            {
	                cursor.rectTransform.localRotation = Quaternion.AngleAxis(GetScreenAngle(r - tracker.ArrowCursor.rectTransform.localPosition), Vector3.forward);
	            }
	            else
	            {
	                cursor.rectTransform.localRotation = Quaternion.identity;
	            }
	        }
	        else
	        {
                tracker.HideAllCursors();
	        }
	    }
	}

	public void AddTracker(Tracker tracker)
	{
		if (_trackers == null)
		{
			_trackers = new List<Tracker>();
		}

		// Arrow
		var arrowImg = CreateTracker(tracker.ArrowCursorImage);
		tracker.ArrowCursor = arrowImg;
		arrowImg.name = tracker.name + "_Arrow";

		// Tracker
		var trackerImg = CreateTracker(tracker.TrackerCurosrImage);
		tracker.TrackerCurosr = trackerImg;
		trackerImg.name = tracker.name + "_Tracker";

        // Far Tracker
        var farTrackerImg = CreateTracker(tracker.FarTrackerCursorImage);
        tracker.FarTrackerCursor = farTrackerImg;
        farTrackerImg.name = tracker.name + "_FarTracker";

        // Very Far Tracker
        var veryFarTrackerImg = CreateTracker(tracker.VeryFarTrackerCursorImage);
        tracker.VeryFarTrackerCursor = veryFarTrackerImg;
        veryFarTrackerImg.name = tracker.name + "_VeryFarTracker";

        // Health Bar Backgorund
        var healthBarBack = CreateTracker(healthBarBackgroundTexture, new Vector2(0.5f, -15f));
        tracker.HealthBarBackground = healthBarBack;
        healthBarBack.name = tracker.name + "_HealthBack";

        // Health Bar
	    var healthBar = CreateTracker(healthBarTexture, new Vector2(0.5f, -15f));
        tracker.HealthBar = healthBar;
        tracker.HealthBar.type = Image.Type.Filled;
        tracker.HealthBar.fillMethod = Image.FillMethod.Horizontal;
	    tracker.HealthBar.fillAmount = 1f;
	    healthBar.name = tracker.name + "_Health";

		_trackers.Add(tracker);
	}

	private Image CreateTracker(Texture2D tex)
	{
		return CreateTracker(tex, new Vector2(0.5f, 0.5f));
	}

    private Image CreateTracker(Texture2D tex, Vector2 pivot)
    {
        var arrowObj = new GameObject();
        var arrowImg = arrowObj.AddComponent<Image>();
        arrowImg.rectTransform.pivot = pivot;
        arrowImg.transform.SetParent(transform);
        arrowImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        arrowImg.SetNativeSize();
        return arrowImg;
    }

	public void RemoveTracker(Tracker tracker)
	{
		_trackers.Remove(tracker);

		if (tracker.TrackerCurosr != null && tracker.TrackerCurosr.gameObject != null)
		{
			Destroy(tracker.TrackerCurosr.gameObject);
		}
        if (tracker.FarTrackerCursor != null && tracker.FarTrackerCursor.gameObject != null)
        {
            Destroy(tracker.FarTrackerCursor.gameObject);
        }
        if (tracker.VeryFarTrackerCursor != null && tracker.VeryFarTrackerCursor.gameObject != null)
        {
            Destroy(tracker.VeryFarTrackerCursor.gameObject);
        }
		if (tracker.ArrowCursor != null && tracker.ArrowCursor.gameObject != null)
		{
			Destroy(tracker.ArrowCursor.gameObject);
		}
        if (tracker.HealthBarBackground != null && tracker.HealthBarBackground.gameObject != null)
        {
            Destroy(tracker.HealthBarBackground.gameObject);
        }
        if (tracker.HealthBar != null && tracker.HealthBar.gameObject != null)
        {
            Destroy(tracker.HealthBar.gameObject);
        }
	}

	private float GetScreenAngle(Vector2 point)
	{
		var delta = point - screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
