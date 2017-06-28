using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public Text CreditText;

    [Header("Left Panel")]
    public Transform LeftPanel;
    public HangarWeaponButton WeaponButtonPrefab;

    [Header("Vehicle Preview")]
    public Text VehicleNameText;
    public Transform VehicleViewTransform;

    [Header("Right Panel")]
    public Transform RightPanel;
    public HangarBluePrintButton BluePrintButtonPrefab;
    public HangarOwnedBluePrintButton OwnedBluePrintButtonPrefab;

    private int _vehicleIndex;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        UpdateCredits(playerFile.SpaceJunk);

        var primaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Primary);
        var secondaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Secondary);

        _vehicleIndex = VehiclePool.Current.Vehicles.IndexOf(VehiclePool.ByKey(playerFile.Ship));
        ShowVehicle(_vehicleIndex);

        UpdateLeftBar(primaryWeapon, secondaryWeapon);
        UpdateRightBar();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void ShowVehicle(int index)
    {
        foreach(Transform t in VehicleViewTransform)
        {
            Destroy(t.gameObject);
        }
        var vehiclePrefab = VehiclePool.Current.Vehicles[index];
        var vehiclePreview = Utility.InstantiateInParent(vehiclePrefab.PreviewPrefab, VehicleViewTransform);
        VehicleNameText.text = vehiclePrefab.Name;

        var playerFile = PlayerFile.ReadFromFile(PlayerFile.Filename);
        playerFile.Ship = vehiclePrefab.name;
        playerFile.WriteToFile(PlayerFile.Filename);
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
        var weapon = bluePrint.Item as WeaponDefinition;
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
        var primaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Primary);
        var secondaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Secondary);
        UpdateLeftBar(primaryWeapon, secondaryWeapon);
    }

    private WeaponDefinition GetEquippedWeapon(PlayerFile playerFile, PlayerFile.EquippedSlot slot)
    {
        return BluePrintPool.ByKey<WeaponDefinition>(playerFile.GetItemIn(slot).Key).Item as WeaponDefinition;
    }

    private void UpdateCredits(int creditCount)
    {
        CreditText.text = string.Format("{0:N0}c", creditCount);
    }

    public void NextVehicle()
    {
        _vehicleIndex++;
        if (_vehicleIndex > VehiclePool.Current.Vehicles.Count - 1)
            _vehicleIndex = 0;
        ShowVehicle(_vehicleIndex);
    }

    public void PreviousVehicle()
    {
        _vehicleIndex--;
        if (_vehicleIndex < 0)
            _vehicleIndex = VehiclePool.Current.Vehicles.Count - 1;
        ShowVehicle(_vehicleIndex);
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
