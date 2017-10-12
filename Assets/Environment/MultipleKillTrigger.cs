using System.Collections.Generic;
using UnityEngine;

public class MultipleKillTrigger : MonoBehaviour
{
    public List<Killable> Killables;
    public Triggerable OnInitializeTrigger;
    public Triggerable OnAllKilledTrigger;
    public VehicleTrackerValues TrackerValues;

    private int _triggerDieCount;
    private int _dieCount;

    public void Initialize()
    {
        foreach (var killable in Killables)
        {
            var tracker = killable.gameObject.GetComponentInChildren<TurretTracker>();
            TrackerManager.Current.RemoveTracker(tracker);
            tracker.DestroyInstance();

            var objectiveTracker = tracker.gameObject.AddComponent<VehicleTracker>();
            objectiveTracker.Options = TrackerValues;

            killable.OnDie += OnKillableDie;
        }
        _dieCount = 0;
        _triggerDieCount = Killables.Count;

        if (OnInitializeTrigger != null)
            OnInitializeTrigger.Trigger();
    }

    private void OnKillableDie(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        _dieCount++;
        sender.OnDie -= OnKillableDie;
        if (_dieCount == _triggerDieCount)
        {
            if (OnAllKilledTrigger != null)
                OnAllKilledTrigger.Trigger();
        }
    }
}
