using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public Text CreditText;
    public Transform LeftPanel;
    public HangarWeaponButton WeaponButtonPrefab;
    public Transform RightPanel;

    private List<PlayerFile.InventoryItem> _inventory;
    //private List<WeaponDefinition> _inventory;

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
        _inventory = playerFile.Inventory;//.Select(i => WeaponDefinitionPool.ByKey(BluePrintPool.ByKey(i.Key).Weapon.Key)).ToList();
        UpdateRightBar();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void UpdateLeftBar(WeaponDefinition primaryWeapon, WeaponDefinition secondaryWeapon)
    {
        foreach(Transform item in LeftPanel)
        {
            Destroy(item.gameObject);
        }

        var primaryWeaponButton = Instantiate(WeaponButtonPrefab, LeftPanel);
        primaryWeaponButton.Bind(primaryWeapon, null);

        var secondaryWeaponButton = Instantiate(WeaponButtonPrefab, LeftPanel);
        secondaryWeaponButton.Bind(secondaryWeapon, null);
    }

    private void UpdateRightBar()
    {
        foreach (Transform item in RightPanel)
        {
            Destroy(item.gameObject);
        }

        /*
        foreach (var item in _inventory)
        {
            var itemButton = Instantiate(WeaponButtonPrefab, RightPanel);
            itemButton.Bind(item, (weapon) =>
            {
                AssignWeapon(weapon);
                UpdateRightBar();
            });
        }
        */
        foreach(var item in _inventory)
        {
            var itemButton = Instantiate(WeaponButtonPrefab, RightPanel);
            var thing = WeaponDefinitionPool.ByKey(BluePrintPool.ByKey(item.Key).Weapon.Key);
            itemButton.Bind(thing, (weapon) =>
            {
                AssignWeapon(weapon);
                UpdateRightBar();
            });
        }
    }

    private void AssignWeapon(WeaponDefinition weapon)
    {
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        if (weapon.Type == ItemType.PrimaryWeapon)
        {
            var oldKey = playerFile.PrimaryWeaponKey;
            playerFile.PrimaryWeaponKey = weapon.Key;
            /*
            _inventory.Remove(weapon);
            _inventory.Add(WeaponDefinitionPool.ByKey(oldKey));
            */
        }
        if (weapon.Type == ItemType.SecondaryWeapon)
        {
            var oldKey = playerFile.SecondaryWeaponKey;
            playerFile.SecondaryWeaponKey = weapon.Key;
            /*
            _inventory.Remove(weapon);
            _inventory.Add(WeaponDefinitionPool.ByKey(oldKey));
            */
        }
        playerFile.Inventory = _inventory;//.Select(i => i.Key).ToList();
        playerFile.WriteToFile(PlayerFile.Filename);
        UpdateLeftBar(WeaponDefinitionPool.ByKey(playerFile.PrimaryWeaponKey), WeaponDefinitionPool.ByKey(playerFile.SecondaryWeaponKey));
    }

    private void UpdateCredits(int creditCount)
    {
        CreditText.text = string.Format("{0:N0}c", creditCount);
    }

    public void GotoGalaxyMap()
    {
        SceneManager.LoadScene("GalaxyMap");
    }

    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }
}
