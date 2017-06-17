using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionCompleteScreen : MonoBehaviour
{
    public CanvasGroup ScreenGroup;
    public Text KillsText;
    public Text CreditsText;

    private static MissionCompleteScreen _current;

    public static MissionCompleteScreen Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
        Hide();
    }

    public void Show(int playerKills)
    {
        ScreenGroup.alpha = 1f;
        var delay = 0.5f;
        StartCoroutine(DelayedAction(delay, () => {
            KillsText.enabled = true;
            KillsText.text = string.Format("Kills: {0}", playerKills);
        }));
        delay += 0.5f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            CreditsText.enabled = true;
            CreditsText.text = string.Format("Credits Earned: {0}", 0);
        }));
        delay += 0.5f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            StartCoroutine(SumCredits(playerKills));
        }));
    }

    public void Hide()
    {
        ScreenGroup.alpha = 0;
        KillsText.enabled = false;
    }

    private IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator SumCredits(int playerKills)
    {
        var tickTime = 0.05f;
        var kills = playerKills;
        var credits = 0;
        for (var i = 0; i < playerKills; i++)
        {
            yield return new WaitForSeconds(tickTime);
            kills--;
            credits += 10;
            KillsText.text = string.Format("Kills: {0}", kills);
            CreditsText.text = string.Format("Credits Earned: {0}", credits);
        }
    }
}
