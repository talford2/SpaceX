using System.Collections;
using UnityEngine;

public class TriggerableMessage : Triggerable
{
    public float Delay;
    public string Author;
    [Multiline]
    public string Message;

    public override void Trigger(float delay = 0)
    {
        StartCoroutine(DisplayMessage(Delay));
    }

    private IEnumerator DisplayMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        CommMessaging.Current.ShowMessage(Player.Current.VehicleInstance.gameObject, Author, Message);
    }
}
