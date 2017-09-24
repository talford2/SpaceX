using System.Collections;
using UnityEngine;

public class DelaySpawnTrigger : Triggerable
{
    public bool StartOn;
    public PirateSquad TriggerEvent;
    public float Delay;

    public Triggerable TriggerOnDefeat;

    private void Start()
    {
        TriggerEvent.OnAllDied += OnAllDead;
        if (StartOn)
            Trigger(Delay);
    }

    private IEnumerator DelayedTrigger(PirateSquad triggerable, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerable.Trigger();
    }

    private void OnAllDead()
    {
        TriggerEvent.OnAllDied -= OnAllDead;
        TriggerOnDefeat.Trigger();
    }

    public override void Trigger(float delay = 0)
    {
        StartCoroutine(DelayedTrigger(TriggerEvent, Delay));
    }
}
