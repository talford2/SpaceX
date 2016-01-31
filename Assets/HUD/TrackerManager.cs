using UnityEngine;
using System.Collections.Generic;

public class TrackerManager : MonoBehaviour {
    private static TrackerManager current;

    public static TrackerManager Current
    {
        get { return current; }
    }

    private List<Tracker> trackers;
    private Canvas trackerCanvas;

    private void Awake()
    {
        trackerCanvas = GetComponentInChildren<Canvas>();
        trackers = new List<Tracker>();
        current = this;
    }

    private void Start()
    {
        Universe.Current.ViewPort.OnMove += UpdateTrackers;
    }

    public void AddTracker(Tracker tracker)
    {
        trackers.Add(tracker);
        var trackerImage = tracker.CreateInstance();
        trackerImage.transform.SetParent(trackerCanvas.transform);
    }

    public void RemoveTracker(Tracker enemyTracker)
    {
        trackers.Remove(enemyTracker);
    }

    public List<Tracker> GetTrackers()
    {
        return trackers;
    }

    private void UpdateTrackers()
    {
        foreach (var tracker in trackers)
        {
            tracker.UpdateInstance();
        }
    }

    public void SetTrackersVisibility(bool value)
    {
        trackerCanvas.enabled = value;
    }
}
