using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public Text CreditText;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        UpdateCredits(playerFile.SpaceJunk);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void UpdateCredits(int creditCount)
    {
        CreditText.text = string.Format("{0}c", creditCount);
    }
}
