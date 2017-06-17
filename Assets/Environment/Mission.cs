using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission : MonoBehaviour
{
    private static Mission _current;

    public static Mission Current { get { return _current; } }

    private int _playerKillCount;

    private void Awake()
    {
        _current = this;
        _playerKillCount = 0;
    }

    public void TriggerFinish(float delay)
    {
        StartCoroutine(DelayedFinish(delay));
    }

    public void IncrementKills(int amount)
    {
        _playerKillCount += amount;
    }

    private IEnumerator DelayedFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        MissionCompleteScreen.Current.Show(_playerKillCount);
        StartCoroutine(DelayedSceneLoad(5f));
    }

    private IEnumerator DelayedSceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
