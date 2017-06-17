using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionCompleteScreen : MonoBehaviour
{
    public CanvasGroup ScreenGroup;
    public Text TimeText;
    public Text KillsText;

    private static MissionCompleteScreen _current;

    public static MissionCompleteScreen Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
        Hide();
    }

    public void Show()
    {
        ScreenGroup.alpha = 1f;
        StartCoroutine(DelayedAction(0.5f, () => { TimeText.enabled = true; }));
        StartCoroutine(DelayedAction(1f, () => { KillsText.enabled = true; }));
    }

    public void Hide()
    {
        ScreenGroup.alpha = 0;
        TimeText.enabled = false;
        KillsText.enabled = false;
    }

    private IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
