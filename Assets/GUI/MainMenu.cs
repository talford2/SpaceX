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
                SceneManager.LoadScene("Battle");
                break;
            case "Campaign":
                SceneManager.LoadScene("Test1");
                break;
            case "Exit":
                Application.Quit();
                break;
        }
    }
}
