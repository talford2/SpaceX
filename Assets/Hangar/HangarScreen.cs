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
    public HangarBluePrintButton BluePrintButtonPrefab;
    public HangarOwnedBluePrintButton OwnedBluePrintButtonPrefab;
    public Transform RightPanel;

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

        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        var sortedInventory = playerFile.Inventory.OrderBy(i => i.SortStatus());

        foreach (var item in sortedInventory.Where(i=>i.EquippedSlot == PlayerFile.EquippedSlot.Inventory))
        {
            var bluePrint = BluePrintPool.ByKey(item.Key);
            if (item.IsOwned)
            {
                var itemButton = Instantiate(OwnedBluePrintButtonPrefab, RightPanel);
                itemButton.Bind(item,
                    (button, inventoryItem) =>
                    {
                        AssignWeapon(inventoryItem);
                        UpdateRightBar();
                    });
            }
            else
            {
                var itemButton = Instantiate(BluePrintButtonPrefab, RightPanel);
                itemButton.Bind(item,
                    (button, inventoryItem) =>
                    {
                        var btnPlayerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
                        var btnBluePrint = BluePrintPool.ByKey(inventoryItem.Key);
                        if (btnPlayerFile.SpaceJunk >= btnBluePrint.Price)
                        {
                            BuyWeapon(btnPlayerFile, btnBluePrint);
                            UpdateCredits(btnPlayerFile.SpaceJunk);
                            UpdateRightBar();
                        }
                    },
                    (button, inventoryItem) =>
                    {
                        AssignWeapon(inventoryItem);
                        UpdateRightBar();
                    });
                itemButton.SetAffordable(playerFile.SpaceJunk >= bluePrint.Price);
            }
        }
    }
    
    private void BuyWeapon(PlayerFile playerFile, BluePrint bluePrint)
    {
        playerFile.Inventory.First(i => i.Key == bluePrint.Key).IsOwned = true;
        playerFile.SpaceJunk -= bluePrint.Price;
        playerFile.WriteToFile(PlayerFile.Filename);
    }

    private void AssignWeapon(PlayerFile.InventoryItem item)
    {
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        var bluePrint = BluePrintPool.ByKey(item.Key);
        var weapon = bluePrint.Weapon;
        if (weapon.Type == ItemType.PrimaryWeapon)
        {
            playerFile.GetItemIn(PlayerFile.EquippedSlot.Primary).EquippedSlot = PlayerFile.EquippedSlot.Inventory;
            playerFile.GetItemByKey(item.Key).EquippedSlot = PlayerFile.EquippedSlot.Primary;
        }
        if (weapon.Type == ItemType.SecondaryWeapon)
        {
            playerFile.GetItemIn(PlayerFile.EquippedSlot.Secondary).EquippedSlot = PlayerFile.EquippedSlot.Inventory;
            playerFile.GetItemByKey(item.Key).EquippedSlot = PlayerFile.EquippedSlot.Secondary;
        }
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
