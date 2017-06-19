using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionCompleteScreen : MonoBehaviour
{
    public CanvasGroup ScreenGroup;
    public Text HeaderText;
    public Text KillsText;
    public Text EarnedCreditsText;
    public Text TotalCreditsText;

    [Header("Reward Item")]
    public CanvasGroup ItemPanel;
    public Image ItemIcon;
    public Text ItemName;

    [Header("Sounds")]
    public AudioClip TriggerSound;
    public AudioClip TextAppearSound;
    public AudioClip CreditTickSound;

    private bool _canEscape;

    private static MissionCompleteScreen _current;
    public static MissionCompleteScreen Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
        _canEscape = false;
        Hide();
    }

    private void Update()
    {
        if (_canEscape)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                SceneManager.LoadScene("GalaxyMap");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            var viewPort = Universe.Current.ViewPort.transform;
            ResourcePoolManager
                .GetAvailable(ResourcePoolIndex.AnonymousSound, viewPort.position, Quaternion.identity)
                .GetComponent<AnonymousSound>().PlayAt(clip, viewPort.position, 1f, false);
        }
    }

    public void Show(int playerKills)
    {
        ScreenGroup.alpha = 1f;
        MusicPlayer.Current.TriggerFadeOut(1f);
        PlaySound(TriggerSound);
        var delay = 0.5f;
        StartCoroutine(FadeIn(delay));
        delay += 1f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            KillsText.enabled = true;
            KillsText.text = string.Format("Kills: {0:N0}", playerKills);
            PlaySound(TextAppearSound);
        }));
        delay += 0.5f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            EarnedCreditsText.enabled = true;
            EarnedCreditsText.text = string.Format("Credits Earned: {0:N0}", Mission.Current.GetEarnedCredits());
            PlaySound(TextAppearSound);
        }));
        delay += 0.5f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            TotalCreditsText.enabled = true;
            TotalCreditsText.text = string.Format("TOTAL CREDITS: {0:N0}", PlayerController.Current.SpaceJunkCount - Mission.Current.GetEarnedCredits());
            PlaySound(TextAppearSound);
            _canEscape = true;
        }));
        delay += 1f;
        StartCoroutine(DelayedAction(delay, () =>
        {
            StartCoroutine(SumCredits(Mission.Current.GetEarnedCredits()));
        }));
        var bluePrint = GetRandomBluePrint();
        if (bluePrint != null)
        {
            var weapon = WeaponDefinitionPool.ByKey(bluePrint.Weapon.Key);
            PlayerController.Current.Give(new PlayerFile.InventoryItem() { Key = bluePrint.Key });
            delay += 0.5f;
            StartCoroutine(DelayedAction(delay, () =>
            {
                ItemPanel.alpha = 1f;
                ItemIcon.sprite = weapon.InventorySprite;
                ItemName.text = weapon.Name;
                PlaySound(TextAppearSound);
            }));
        }
    }

    public void Hide()
    {
        ScreenGroup.alpha = 0f;
        HeaderText.enabled = false;
        KillsText.enabled = false;
        EarnedCreditsText.enabled = false;
        TotalCreditsText.enabled = false;
        ItemPanel.alpha = 0f;
    }

    private IEnumerator FadeIn(float time)
    {
        var interval = 0.01f;
        for (var t = 0f; t < time; t += interval)
        {
            yield return new WaitForSeconds(interval);
            var fraction = Mathf.Clamp01(t / time);
            ScreenGroup.alpha = fraction;
        }
        ScreenGroup.alpha = 1f;
        HeaderText.enabled = true;
    }

    private IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    private IEnumerator SumCredits(int earnedCredits)
    {
        var totalTime = 1f;
        var tickTime = totalTime / earnedCredits;

        var totalCredits = PlayerController.Current.SpaceJunkCount - earnedCredits;
        for (var credits = earnedCredits; credits >= 0; credits--)
        {
            yield return new WaitForSeconds(tickTime);
            EarnedCreditsText.text = string.Format("Credits Earned: {0:N0}", credits);
            TotalCreditsText.text = string.Format("TOTAL CREDITS: {0:N0}", totalCredits);
            PlaySound(CreditTickSound);
            totalCredits++;
        }
    }

    private BluePrint GetRandomBluePrint()
    {
        var bluePrints = BluePrintPool.All();
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        foreach (var item in playerFile.Inventory.Where(i => i.BluePrintsOwned == BluePrintPool.ByKey(i.Key).RequiredCount || i.IsOwned))
        {
            bluePrints.RemoveAll(b => b.Key == item.Key);
        }
        if (!bluePrints.Any())
            return null;
        return bluePrints[UnityEngine.Random.Range(0, bluePrints.Count)];
    }
}
