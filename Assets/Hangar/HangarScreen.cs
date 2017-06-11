using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public Text CreditText;
    public Text PrimaryWeaponText;
    public Text SecondaryWeaponText;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        UpdateCredits(playerFile.SpaceJunk);
        UpdateLeftBar(WeaponDefinitionPool.ByKey(playerFile.PrimaryWeaponKey), WeaponDefinitionPool.ByKey(playerFile.SecondaryWeaponKey));
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void UpdateLeftBar(WeaponDefinition primaryWeapon, WeaponDefinition secondaryWeapon)
    {
        PrimaryWeaponText.text = primaryWeapon.Name;
        SecondaryWeaponText.text = secondaryWeapon.Name;
    }

    private void UpdateCredits(int creditCount)
    {
        CreditText.text = string.Format("{0}c", creditCount);
    }
}
