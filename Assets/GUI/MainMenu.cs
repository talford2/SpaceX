using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ButtonCommand(string command)
    {
        Debug.Log("You clicked: " + command);
        switch (command)
        {
            case "Battle":
                LoadWithLoader("Battle");
                break;
            case "Campaign":
                LoadWithLoader("Test1");
                break;
            case "Hangar":
                LoadWithLoader("Hangar2");
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }

    private void LoadWithLoader(string sceneName)
    {
        PlayerContext.Current.SceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
