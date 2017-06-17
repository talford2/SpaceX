using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionCompleteScreen : MonoBehaviour
{
    public CanvasGroup ScreenGroup;
    public Text KillsText;
    public Text EarnedCreditsText;
    public Text TotalCreditsText;

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
            EarnedCreditsText.enabled = true;
            EarnedCreditsText.text = string.Format("Credits Earned: {0}", Mission.Current.GetEarnedCredits());
        }));
        delay += 1f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            StartCoroutine(SumCredits(playerKills));
        }));
    }

    public void Hide()
    {
        ScreenGroup.alpha = 0;
        KillsText.enabled = false;
        EarnedCreditsText.enabled = false;
        TotalCreditsText.enabled = false;
    }

    private IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator SumCredits(int earnedCredits)
    {
        var tickTime = 0.05f;
        var credits = earnedCredits;
        var totalCredits = PlayerController.Current.SpaceJunkCount - earnedCredits;
        for (var i = 0; i < earnedCredits; i++)
        {
            yield return new WaitForSeconds(tickTime);
            credits--;
            totalCredits++;
            EarnedCreditsText.text = string.Format("Credits Earned: {0}", credits);
            TotalCreditsText.text = string.Format("TOTAL CREDITS: {0}", totalCredits);
        }
    }
}
