using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HangarScreen : MonoBehaviour
{
    public CoverScreen Cover;
    public Text CreditText;

    [Header("Left Panel")]
    public Transform LeftPanel;
    public HangarWeaponButton WeaponButtonPrefab;

    [Header("Vehicle Preview")]
    public Text VehicleNameText;
    public Button PreviousButton;
    public Button NextButton;
    public Transform VehicleViewTransform;
    public Color GreyOut;
    public Text VehicleProgressText;

    [Header("Right Panel")]
    public Transform RightPanel;
    public HangarBluePrintButton BluePrintButtonPrefab;
    public HangarOwnedBluePrintButton OwnedBluePrintButtonPrefab;

    private List<Vehicle> _vehicles;
    private int _vehicleIndex;

    private string _sceneName;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        var playerFile = PlayerFile.ReadFromFile();
        UpdateCredits(playerFile.SpaceJunk);

        var primaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Primary);
        var secondaryWeapon = GetEquippedWeapon(playerFile, PlayerFile.EquippedSlot.Secondary);

        _vehicles = GetPlayersVehicles();
        //_vehicleIndex = _vehicles.Select((vehicle, index) => new { vehicle, index }).First(i => i.vehicle.Key == playerFile.Ship).index;
        _vehicleIndex = GetShipIndex(playerFile.Ship, _vehicles);

        var multipleVehicles = _vehicles.Count > 1;
        NextButton.gameObject.SetActive(multipleVehicles);
        PreviousButton.gameObject.SetActive(multipleVehicles);

        ShowVehicle(_vehicleIndex);

        UpdateLeftBar(primaryWeapon, secondaryWeapon);
        UpdateRightBar();

        Cover.TriggerFadeOut();
    }

    private int GetShipIndex(string shipKey, List<Vehicle> vehicles)
    {
        for (var i = 0; i < vehicles.Count; i++)
        {
            if (_vehicles[i].Key == shipKey)
            {
                return i;
            }
        }
        return -1;
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
        var vehiclePrefab = _vehicles[index];
        var vehiclePreview = Utility.InstantiateInParent(vehiclePrefab.PreviewPrefab, VehicleViewTransform);
        vehiclePreview.transform.localPosition = vehiclePrefab.PreviewPrefab.transform.localPosition;
        VehicleNameText.text = vehiclePrefab.Name;

        var playerFile = PlayerFile.ReadFromFile();
        playerFile.Ship = vehiclePrefab.Key;
        var playersShip = playerFile.Ships.First(s => s.Key == playerFile.Ship);
        if (playersShip.IsOwned)
        {
            VehicleNameText.color = Color.white;
            VehicleProgressText.enabled = false;
        }
        else
        {
            VehicleNameText.color = GreyOut;
            VehicleProgressText.color = GreyOut;
            VehicleProgressText.text = string.Format("{0} / {1}", playersShip.BluePrintsOwned, BluePrintPool.ByKey(playerFile.Ship).RequiredCount);
            VehicleProgressText.enabled = true;
        }
        playerFile.WriteToFile();
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

        var playerFile = PlayerFile.ReadFromFile();
        var sortedInventory = playerFile.Inventory.OrderBy(i => i.SortStatus());

        foreach (var item in sortedInventory.Where(i => i.EquippedSlot == PlayerFile.EquippedSlot.Inventory))
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
                        var btnPlayerFile = PlayerFile.ReadFromFile();
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
        playerFile.WriteToFile();
    }

    private void AssignWeapon(PlayerFile.InventoryItem item)
    {
        var playerFile = PlayerFile.ReadFromFile();
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
        playerFile.WriteToFile();
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

    private List<Vehicle> GetPlayersVehicles()
    {
        var vehicles = new List<Vehicle>();
        var playerFile = PlayerFile.ReadFromFile();
        playerFile.Ships.ForEach(s => vehicles.Add(VehiclePool.ByKey(s.Key)));
        return vehicles;
    }

    public void NextVehicle()
    {
        _vehicleIndex++;
        if (_vehicleIndex > _vehicles.Count - 1)
            _vehicleIndex = 0;
        ShowVehicle(_vehicleIndex);
    }

    public void PreviousVehicle()
    {
        _vehicleIndex--;
        if (_vehicleIndex < 0)
            _vehicleIndex = _vehicles.Count - 1;
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

    private void LoadWithLoader()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
