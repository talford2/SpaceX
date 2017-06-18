using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public Text CreditText;
    public Transform LeftPanel;
    public HangarWeaponButton WeaponButtonPrefab;
    public HangarBluePrintButton BlueprintButtonPrefab;
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

        var primaryWeapon = BluePrintPool.ByKey(playerFile.GetItemIn(PlayerFile.EquippedSlot.Primary).Key).Weapon;
        var secondaryWeapon = BluePrintPool.ByKey(playerFile.GetItemIn(PlayerFile.EquippedSlot.Secondary).Key).Weapon;

        UpdateLeftBar(primaryWeapon, secondaryWeapon);
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
        Debug.Log("UPDATE RIGHT BAR!");
        foreach (Transform item in RightPanel)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in _inventory)
        {
            var itemButton = Instantiate(BlueprintButtonPrefab, RightPanel);
            itemButton.Bind(item,
                (button, inventoryItem) =>
                {
                    var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
                    var bluePrint = BluePrintPool.ByKey(inventoryItem.Key);
                    if (playerFile.SpaceJunk >= bluePrint.Price)
                    {
                        playerFile.Inventory.First(i => i.Key == bluePrint.Key).IsOwned = true;
                        playerFile.SpaceJunk -= bluePrint.Price;
                        playerFile.WriteToFile(PlayerFile.Filename);
                        UpdateCredits(playerFile.SpaceJunk);
                        button.SetOwned(item);
                        //UpdateRightBar();
                    }
                },
                (button, inventoryItem) =>
                {
                    AssignWeapon(inventoryItem);
                    UpdateRightBar();
                });
        }
    }

    private void AssignWeapon(PlayerFile.InventoryItem item)
    {
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weapon = bluePrint.Weapon;
        if (weapon.Type == ItemType.PrimaryWeapon)
        {
            playerFile.GetItemIn(PlayerFile.EquippedSlot.Primary).EquippedSlot = PlayerFile.EquippedSlot.Inventory;
            item.EquippedSlot = PlayerFile.EquippedSlot.Primary;

        }
        if (weapon.Type == ItemType.SecondaryWeapon)
        {
            playerFile.GetItemIn(PlayerFile.EquippedSlot.Secondary).EquippedSlot = PlayerFile.EquippedSlot.Inventory;
            item.EquippedSlot = PlayerFile.EquippedSlot.Secondary;
        }
        playerFile.Inventory = _inventory;
        playerFile.WriteToFile(PlayerFile.Filename);
        var primaryWeapon = BluePrintPool.ByKey(playerFile.GetItemIn(PlayerFile.EquippedSlot.Primary).Key).Weapon;
        var secondaryWeapon = BluePrintPool.ByKey(playerFile.GetItemIn(PlayerFile.EquippedSlot.Secondary).Key).Weapon;
        UpdateLeftBar(primaryWeapon, secondaryWeapon);
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
