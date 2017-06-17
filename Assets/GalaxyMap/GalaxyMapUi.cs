using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalaxyMapUi : MonoBehaviour
{
    private static GalaxyMapUi _current;
    public static GalaxyMapUi Current { get { return _current; } }

    public CanvasGroup EnterSystemPanel;

    private void Awake()
    {
        _current = this;
        SetVisibleEnterSystem(false);
    }

    public void SetVisibleEnterSystem(bool value)
    {
        EnterSystemPanel.gameObject.SetActive(value);
    }

    public void GotoHangar()
    {
        SceneManager.LoadScene("Hangar2");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }
}
