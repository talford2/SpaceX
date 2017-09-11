using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle YAxisToggle;

    private void Awake()
    {
        if (OptionsFile.Exists())
        {
            var optionsFile = OptionsFile.ReadFromFile();
            YAxisToggle.isOn = optionsFile.InvertYAxis;
        }
        YAxisToggle.onValueChanged.AddListener(OnyAxisChange);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnyAxisChange(bool value)
    {
        var optionsFile = OptionsFile.ReadOrNew();
        optionsFile.InvertYAxis = value;
        optionsFile.WriteToFile();
    }
}
