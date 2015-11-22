using UnityEngine;
using System.Collections;

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
        QuitMenu.SetActive(true);
    }

    public void HideQuitMenu()
    {
        QuitMenu.SetActive(false);
    }

    public void ToggleQuitMenu()
    {
        var isActive = !QuitMenu.activeSelf;
        QuitMenu.SetActive(isActive);
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
