using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public GameObject QuitMenu;

    private static Menus _current;

    public static Menus Current { get { return _current; } }

    private void Awake()
    {
        QuitMenu.SetActive(false);
        _current = this;
    }

    public void ShowQuitMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        HeadsUpDisplay.Current.HideCrosshair();
        QuitMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideQuitMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HeadsUpDisplay.Current.ShowCrosshair();
        QuitMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleQuitMenu()
    {
        var isActive = !QuitMenu.activeSelf;
        if (isActive)
        {
            ShowQuitMenu();
        }
        else
        {
            HideQuitMenu();
        }
    }

    public void Resume()
    {
        HideQuitMenu();
    }

    public void Restart()
    {
        HideQuitMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }
}
