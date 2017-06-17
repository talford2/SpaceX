using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mission : MonoBehaviour
{
    private static Mission _current;

    public static Mission Current { get { return _current; } }

    private void Awake()
    {
        _current = this;
    }

    public void TriggerFinish(float delay)
    {
        StartCoroutine(DelayedFinish(delay));
    }

    private IEnumerator DelayedFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        MissionCompleteScreen.Current.Show();
        StartCoroutine(DelayedSceneLoad(5f));
    }

    private IEnumerator DelayedSceneLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
