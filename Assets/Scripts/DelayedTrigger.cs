using System.Collections;
using UnityEngine;

public class DelayedTrigger : Triggerable
{
    public bool StartOn = true;
    public float Delay;
    public Triggerable TriggerObject;

    private void Start()
    {
        if (StartOn)
            Trigger();
    }

    public override void Trigger(float delay = 0)
    {
        StartCoroutine(DelayTrigger(Delay, TriggerObject));
    }

    private IEnumerator DelayTrigger(float delay, Triggerable triggerable)
    {
        yield return new WaitForSeconds(delay);
        triggerable.Trigger();
    }
}
