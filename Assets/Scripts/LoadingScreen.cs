using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text LoadingText;

    private bool isLoading;

    private void Start()
    {
        StartCoroutine(LoadScene("Test1"));
    }

    private void Update()
    {
        LoadingText.color = Utility.SetColorAlpha(LoadingText.color, Mathf.PingPong(Time.time, 1f));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        var async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            isLoading = false;
            yield return null;
        }
    }
}
