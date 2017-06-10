using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ButtonCommand(string command)
    {
        Debug.Log("You clicked: " + command);
        switch (command)
        {
            case "Battle":
                PlayerContext.Current.SceneName = "Battle";
                SceneManager.LoadScene("LoadingScene");
                break;
            case "Campaign":
                PlayerContext.Current.SceneName = "Test1";
                SceneManager.LoadScene("LoadingScene");
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }
}
