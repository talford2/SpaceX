using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle YAxisToggle;
    public Toggle MusicToggle;

    private void Awake()
    {
        if (OptionsFile.Exists())
        {
            var optionsFile = OptionsFile.ReadFromFile();
            YAxisToggle.isOn = optionsFile.InvertYAxis;
            MusicToggle.isOn = optionsFile.IsMusicOn;
        }
        YAxisToggle.onValueChanged.AddListener(OnyAxisChange);
        MusicToggle.onValueChanged.AddListener(OnMusicChange);
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

    private void OnMusicChange(bool value)
    {
        var optionsFile = OptionsFile.ReadOrNew();
        optionsFile.IsMusicOn = value;
        optionsFile.WriteToFile();
    }
}
