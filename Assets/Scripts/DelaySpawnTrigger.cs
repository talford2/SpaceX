using System.Collections;
using UnityEngine;

public class DelaySpawnTrigger : MonoBehaviour
{
    public PirateSquad TriggerEvent;
    public float Delay;
    public PirateSquad TriggerOnDefeat;

    private void Start()
    {
        TriggerEvent.OnAllDied += OnAllDead;
        StartCoroutine(DelayedTrigger(TriggerEvent, Delay));
    }

    private IEnumerator DelayedTrigger(PirateSquad triggerable, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerable.Trigger();
    }

    private void OnAllDead()
    {
        TriggerEvent.OnAllDied -= OnAllDead;
        if (TriggerOnDefeat != null)
            StartCoroutine(DelayedTrigger(TriggerOnDefeat, 5f));
    }
}
