using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniverseTrackers : MonoBehaviour
{
	private Camera _cam;

	private Vector3 screenCentre;
	private float boundaryPadding = 20f;
	private Rect screenBounds;

	private Image cursor;
	public Image ArrowCursorPrefab;
	public Image TrackerCurosrPrefab;

	private static Image _arrowPref;
	private static Image _cursorPref;

	private static List<Tracker> _trackers { get; set; }

	private void Awake()
	{
		_arrowPref = ArrowCursorPrefab;
		_cursorPref = TrackerCurosrPrefab;

		_arrowPref.enabled = false;
		_cursorPref.enabled = false;
	}

	private void Start()
	{
		screenCentre = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);

		var viewport = Universe.Current.ViewPort;
		_cam = viewport.GetComponent<Camera>();
		viewport.OnMove += ViewPortMove;
	}

	private void ViewPortMove()
	{
		foreach (var tracker in _trackers)
		{
			var r = _cam.WorldToScreenPoint(tracker.transform.position); // Target.position);
			var boundsPoint = r;
			if (boundsPoint.z < 0f)
				boundsPoint *= -1f;
			var inBounds = screenBounds.Contains(boundsPoint);

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

			cursor.rectTransform.localPosition = Utility.GetBoundsIntersection(r, screenBounds);//, screenCentre);

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

	public static void AddTracker(Tracker tracker)
	{
		if (_trackers == null)
		{
			_trackers = new List<Tracker>();
		}

		tracker.ArrowCursor = Instantiate<Image>(_arrowPref);
		tracker.ArrowCursor.transform.SetParent(_arrowPref.transform.parent, false);
		if (tracker.ArrowCursorImage != null)
		{
			var rec = _arrowPref.GetPixelAdjustedRect();
			tracker.ArrowCursor.sprite = Sprite.Create(tracker.ArrowCursorImage, new Rect(0, 0, rec.width, rec.height), Vector2.zero);
		}

		tracker.TrackerCurosr = Instantiate<Image>(_cursorPref);
		tracker.TrackerCurosr.transform.SetParent(_cursorPref.transform.parent, false);
		if (tracker.TrackerCurosr != null)
		{
			var rec = _cursorPref.GetPixelAdjustedRect();
			tracker.TrackerCurosr.sprite = Sprite.Create(tracker.TrackerCurosrImage, new Rect(0, 0, rec.width, rec.height), Vector2.zero);
		}

		_trackers.Add(tracker);
	}

	public static void RemoveTracker(Tracker tracker)
	{
		Destroy(tracker.ArrowCursor);
		Destroy(tracker.TrackerCurosr);
		_trackers.Remove(tracker);
	}

	private float GetScreenAngle(Vector3 point)
	{
		if (point.z < 0f)
			point *= -1f;

		var delta = point - screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
