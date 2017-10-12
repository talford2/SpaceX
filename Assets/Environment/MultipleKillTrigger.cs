using System.Collections.Generic;
using UnityEngine;

public class MultipleKillTrigger : MonoBehaviour
{
    public List<Killable> Killables;
    public Triggerable OnAllKilledTrigger;
    public VehicleTrackerValues TrackerValues;

    private int _triggerDieCount;
    private int _dieCount;

    public void Initialize()
    {
        foreach(var killable in Killables)
        {
            var tracker = killable.gameObject.GetComponentInChildren<TurretTracker>();
            TrackerManager.Current.RemoveTracker(tracker);
            tracker.DestroyInstance();
            //TrackerManager.Current.AddTracker(tracker);

            var objectiveTracker = tracker.gameObject.AddComponent<VehicleTracker>();
            objectiveTracker.Options = TrackerValues;


            /*
            if (TrackerValues != null)
            {
                var objectiveTracker = killable.gameObject.AddComponent<VehicleTracker>();
                objectiveTracker.Options = TrackerValues;

                if (killable.gameObject.GetComponentInChildren<Targetable>() == null)
                    Debug.LogWarning("NO TRACKER FOUND!");
                objectiveTracker.Targetable = killable.gameObject.GetComponentInChildren<Targetable>();
                objectiveTracker.Killable = killable;
            }
            */
            killable.OnDie += OnKillableDie;
        }
        _dieCount = 0;
        _triggerDieCount = Killables.Count;
    }

    private void OnKillableDie(Killable sender, Vector3 position, Vector3 normal, GameObject attacker)
    {
        _dieCount++;
        sender.OnDie -= OnKillableDie;
        if (_dieCount == _triggerDieCount)
        {
            Debug.Log("TRIGGER SOMETHING");
            OnAllKilledTrigger.Trigger();
        }
    }
}
