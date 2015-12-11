using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniverseTrackers : MonoBehaviour
{
    public Font CallSignFont;
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
	    _cam = viewport.AttachedCamera;
		viewport.OnMove += ViewPortMove;
	}

	private void ViewPortMove()
	{
	    foreach (var tracker in _trackers)
	    {
	        if (tracker.DelayCooldown >= 0f)
	        {
	            tracker.DelayCooldown -= Time.deltaTime;
	            if (tracker.DelayCooldown < 0f)
	            {
	                tracker.IsDelayComplete = true;
	            }
	        }

	        if (tracker.IsDelayComplete && !tracker.IsDisabled)
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

                var cursorPosition = Utility.GetBoundsIntersection(new Vector2(r.x, r.y), screenBounds);
	            cursor.rectTransform.localPosition = cursorPosition;
                tracker.HealthBar.rectTransform.localPosition = cursorPosition;
                tracker.HealthBarBackground.rectTransform.localPosition = cursorPosition;
                tracker.LockingCursor.rectTransform.localPosition = cursorPosition;
                tracker.LockedCursor.rectTransform.localPosition = cursorPosition;
	            tracker.CallSignText.rectTransform.localPosition = cursorPosition;

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

        tracker.ArrowCursor = CreateTracker(tracker.ArrowCursorImage, tracker.name + "_Arrow");
		tracker.TrackerCurosr = CreateTracker(tracker.TrackerCurosrImage, tracker.name + "_Tracker");
        tracker.FarTrackerCursor = CreateTracker(tracker.FarTrackerCursorImage, tracker.name + "_FarTracker");
        tracker.VeryFarTrackerCursor = CreateTracker(tracker.VeryFarTrackerCursorImage, tracker.name + "_VeryFarTracker");
        tracker.HealthBarBackground = CreateTracker(healthBarBackgroundTexture, new Vector2(0.5f, -15f), tracker.name + "_HealthBack");

        // Health Bar
        tracker.HealthBar = CreateTracker(healthBarTexture, new Vector2(0.5f, -15f), tracker.name + "_Health");
        tracker.HealthBar.type = Image.Type.Filled;
        tracker.HealthBar.fillMethod = Image.FillMethod.Horizontal;
	    tracker.HealthBar.fillAmount = 1f;

        // Locking
        tracker.LockingCursor = CreateTracker(tracker.LockingCursorImage, tracker.name + "_Locking");
        tracker.LockedCursor = CreateTracker(tracker.LockedCursorImage, tracker.name + "_Locked");

        // Call Sign
	    tracker.CallSignText = CreatTrackerLabel(new Vector2(0.5f, 0.5f), tracker.name + "_CallSign");

	    tracker.DelayCooldown = 1f;
	    tracker.IsDelayComplete = false;

		_trackers.Add(tracker);
	}

	private Image CreateTracker(Texture2D tex, string instanceName)
	{
		return CreateTracker(tex, new Vector2(0.5f, 0.5f), instanceName);
	}

    private Image CreateTracker(Texture2D tex, Vector2 pivot, string instanceName)
    {
        var arrowObj = new GameObject();
        var arrowImg = arrowObj.AddComponent<Image>();
        arrowImg.rectTransform.pivot = pivot;
        arrowImg.transform.SetParent(transform);
        arrowImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        arrowImg.SetNativeSize();
        arrowImg.name = instanceName;
        return arrowImg;
    }

    private Text CreatTrackerLabel(Vector2 pivot, string instanceName)
    {
        var obj = new GameObject();
        var txt = obj.AddComponent<Text>();
        txt.rectTransform.pivot = pivot;
        txt.transform.SetParent(transform);
        txt.color = Color.white;
        txt.font = CallSignFont;
        txt.alignment = TextAnchor.UpperCenter;
        txt.SetNativeSize();
        txt.name = instanceName;
        return txt;
    }

	public void RemoveTracker(Tracker tracker)
	{
		_trackers.Remove(tracker);
        DestroyTrackerCursor(tracker.ArrowCursor);
        DestroyTrackerCursor(tracker.TrackerCurosr);
        DestroyTrackerCursor(tracker.FarTrackerCursor);
        DestroyTrackerCursor(tracker.VeryFarTrackerCursor);
        DestroyTrackerCursor(tracker.HealthBarBackground);
        DestroyTrackerCursor(tracker.HealthBar);
        DestroyTrackerCursor(tracker.LockingCursor);
        DestroyTrackerCursor(tracker.LockedCursor);
	}

    private void DestroyTrackerCursor(Image cursor)
    {
        if (cursor != null && cursor.gameObject != null)
        {
            Destroy(cursor.gameObject);
        }
    }

	private float GetScreenAngle(Vector2 point)
	{
		var delta = point - screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
