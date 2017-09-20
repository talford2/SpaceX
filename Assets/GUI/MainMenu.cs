using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup CoverScreen;
    public Button ContinueButton;

    private bool isFadeIn;
    private float coverTime = 0.5f;
    private float coverCooldown;

    private string loadSceneOnFadeComplete;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ContinueButton.gameObject.SetActive(PlayerFile.Exists());
        isFadeIn = true;
        loadSceneOnFadeComplete = string.Empty;
        CoverScreen.alpha = 1f;
        coverCooldown = coverTime;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (coverCooldown >= 0f)
        {
            coverCooldown -= Time.deltaTime;
            var fraction = Mathf.Clamp01(coverCooldown / coverTime);
            CoverScreen.alpha = isFadeIn ? fraction : 1f - fraction;
            if (coverCooldown < 0f)
            {
                CoverScreen.alpha = isFadeIn ? 0f : 1f;
                if (!string.IsNullOrEmpty(loadSceneOnFadeComplete))
                    LoadWithLoader(loadSceneOnFadeComplete);
            }
        }
    }

    public void ButtonCommand(string command)
    {
        Debug.Log("You clicked: " + command);
        switch (command)
        {
            case "Battle":
                TriggerFadeAndLoad("GalaxyMap");
                //LoadWithLoader("GalaxyMap");
                PersistOnLoad.Current.Destroy();
                break;
            case "New":
                NewGame();
                TriggerFadeAndLoad("GalaxyMap");
                //LoadWithLoader("GalaxyMap");
                PersistOnLoad.Current.Destroy();
                break;
            case "Options":
                SceneManager.LoadScene("Options");
                break;
            case "Campaign":
                LoadWithLoader("Test1");
                PersistOnLoad.Current.Destroy();
                break;
            case "Hangar":
                SceneManager.LoadScene("Hangar2");
                PersistOnLoad.Current.Destroy();
                break;
            case "Exit":
                PersistOnLoad.Current.Destroy();
                Application.Quit();
                break;
        }
    }

    private void NewGame()
    {
        PlayerFile.GameStart().WriteToFile();
    }

    private void TriggerFadeAndLoad(string sceneName)
    {
        loadSceneOnFadeComplete = sceneName;
        CoverScreen.alpha = 0f;
        isFadeIn = false;
        coverCooldown = coverTime;
    }

    private void LoadWithLoader(string sceneName)
    {
        PlayerContext.Current.SceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
