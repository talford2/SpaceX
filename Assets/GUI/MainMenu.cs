using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup CoverScreen;
    public Button ContinueButton;

    private float coverTime = 1f;
    private float coverCooldown;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ContinueButton.gameObject.SetActive(PlayerFile.Exists());
        CoverScreen.alpha = 1f;
        coverCooldown = coverTime;
        Debug.Break();
    }

    private void Update()
    {
        if (coverCooldown >=0f)
        {
            coverCooldown -= Time.deltaTime;
            var fraction = coverCooldown / coverTime;
            CoverScreen.alpha = fraction;
            if (coverCooldown < 0f)
                CoverScreen.alpha = 0f;
        }
    }

    public void ButtonCommand(string command)
    {
        Debug.Log("You clicked: " + command);
        switch (command)
        {
            case "Battle":
                LoadWithLoader("GalaxyMap");
                PersistOnLoad.Current.Destroy();
                break;
            case "New":
                NewGame();
                LoadWithLoader("GalaxyMap");
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

    private void LoadWithLoader(string sceneName)
    {
        PlayerContext.Current.SceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
