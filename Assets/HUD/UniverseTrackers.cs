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

	private static List<Tracker> _trackers { get; set; }

	public static UniverseTrackers Current;

	private void Awake()
	{
		Current = this;
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
            var screenPoint = _cam.WorldToScreenPoint(tracker.transform.position);
            var r = screenPoint;
            if (r.z < 0f)
                r *= -1f;
		    r.z = 0f;
			var inBounds = screenBounds.Contains(r);

			if (inBounds)
			{
				tracker.TrackerCurosr.enabled = true;
				tracker.ArrowCursor.enabled = false;
				cursor = tracker.TrackerCurosr;
			}
			else
			{
				tracker.TrackerCurosr.enabled = false;
				tracker.ArrowCursor.enabled = true;
				cursor = tracker.ArrowCursor;
			}

		    var dotThing = Vector3.Dot(tracker.transform.position - Universe.Current.ViewPort.transform.position, Universe.Current.ViewPort.transform.forward);
		    if (dotThing < 0f)
		    {
		        if (inBounds)
		        {
		            Debug.Log("BROKEN TRACKER!!!");
                    Debug.Log("SCREEN POINT: " + screenPoint);
                    Debug.Log("r: " + r);
                    Debug.Break();
		        }
		    }

			cursor.rectTransform.localPosition = Utility.GetBoundsIntersection(new Vector2(r.x, r.y), screenBounds);

			if (!inBounds)
			{
				cursor.rectTransform.localRotation = Quaternion.AngleAxis(GetScreenAngle(r - tracker.ArrowCursor.rectTransform.localPosition), Vector3.forward);
			}
			else
			{
				cursor.rectTransform.localRotation = Quaternion.identity;
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

		_trackers.Add(tracker);
	}

	private Image CreateTracker(Texture2D tex)
	{
		var arrowObj = new GameObject();
		var arrowImg = arrowObj.AddComponent<Image>();
		arrowImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
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
		if (tracker.ArrowCursor != null && tracker.ArrowCursor.gameObject != null)
		{
			Destroy(tracker.ArrowCursor.gameObject);
		}
	}

	private float GetScreenAngle(Vector2 point)
	{
		var delta = point - screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
