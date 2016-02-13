using UnityEngine;
using System.Collections.Generic;

public class TrackerManager : MonoBehaviour {
    private static TrackerManager _current;

    public static TrackerManager Current
    {
        get { return _current; }
    }

    private List<Tracker> _trackers;
    private Canvas _trackerCanvas;

    private void Awake()
    {
        _trackerCanvas = GetComponentInChildren<Canvas>();
        _trackers = new List<Tracker>();
        _current = this;
    }

    private void Start()
    {
        Universe.Current.ViewPort.OnMove += UpdateTrackers;
    }

    public void AddTracker(Tracker tracker)
    {
        _trackers.Add(tracker);
        var trackerImage = tracker.CreateInstance();
        trackerImage.transform.SetParent(_trackerCanvas.transform);
    }

    public void RemoveTracker(Tracker enemyTracker)
    {
        _trackers.Remove(enemyTracker);
    }

    public List<Tracker> GetTrackers()
    {
        return _trackers;
    }

    private void UpdateTrackers()
    {
        foreach (var tracker in _trackers)
        {
            tracker.UpdateInstance();
        }
    }

    public void SetTrackersVisibility(bool value)
    {
        _trackerCanvas.enabled = value;
    }
}
