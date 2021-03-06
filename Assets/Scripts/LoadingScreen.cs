﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text LoadingText;
    public Image Loader;

    public float LoaderSpinSpeed;
    private bool isLoading;

    private float period = 180f;
    private float theta;

    private void Start()
    {
        StartCoroutine(LoadScene(PlayerContext.Current.SceneName));
    }

    private void Update()
    {
        theta += period * Mathf.Deg2Rad * Time.deltaTime;
        var frac = 0.9f * Sin01(theta) + 0.1f;
        LoadingText.color = Utility.SetColorAlpha(LoadingText.color, frac);

        Loader.rectTransform.Rotate(Vector3.forward * Time.deltaTime * 360 * LoaderSpinSpeed);
    }

    private float Sin01(float angle)
    {
        return 0.5f * Mathf.Sin(angle) + 0.5f;
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
