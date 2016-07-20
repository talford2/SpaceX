using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour
{
    public Canvas Canvas;
    public Text CallSignText;
    public Text CreditsText;
    public Text KiaText;

    [Header("Equipped Panel")]
    public Button PrimaryButton;
    public Button SecondaryButton;
    public Button ShieldButton;
    public Button EngineButton;
    public Sprite EmptyIcon;

    [Header("Item Panel")]
    public Text ItemNameText;

    [Header("Primary Panel")]
    public CanvasGroup PrimaryPanel;
    public Text PrimaryDamageCostText;
    public Image PrimaryDamageBar;
    public Text PrimaryDamageValueText;
    public Button PrimaryAddDamageButton;

    public Text PrimaryFireRateCostText;
    public Image PrimaryFireRateBar;
    public Text PrimaryFireRateValueText;
    public Button PrimaryAddFireRateButton;

    public Text PrimaryCoolingRateCostText;
    public Image PrimaryCoolingRateBar;
    public Text PrimaryCoolingRateValueText;
    public Button PrimaryAddCoolingRateButton;

    public Text PrimaryHeatCapacityCostText;
    public Image PrimaryHeatCapacityBar;
    public Text PrimaryHeatCapacityValueText;
    public Button PrimaryAddHeatCapacityButton;

    [Header("Secondary Panel")]
    public CanvasGroup SecondaryPanel;

    public Text SecondaryDamageCostText;
    public Image SecondaryDamageBar;
    public Text SecondaryDamageValueText;
    public Button SecondaryAddDamageButton;

    public Text SecondaryFireRateCostText;
    public Image SecondaryFireRateBar;
    public Text SecondaryFireRateValueText;
    public Button SecondaryAddFireRateButton;

    public Text SecondaryCoolingRateCostText;
    public Image SecondaryCoolingRateBar;
    public Text SecondaryCoolingRateValueText;
    public Button SecondaryAddCoolingRateButton;

    public Text SecondaryHeatCapacityCostText;
    public Image SecondaryHeatCapacityBar;
    public Text SecondaryHeatCapacityValueText;
    public Button SecondaryAddHeatCapacityButton;

    [Header("Shield Panel")]
    public CanvasGroup ShieldPanel;

    public Text ShieldCapacityCostText;
    public Image ShieldCapacityBar;
    public Text ShieldCapacityValueText;
    public Button ShieldAddCapacityButton;

    public Text ShieldRegenerationCostText;
    public Image ShieldRegenerationBar;
    public Text ShieldRegenerationValueText;
    public Button ShieldAddRegenerationButton;

    [Header("Engine Panel")]
    public CanvasGroup EnginePanel;

    [Header("Inventory Panel")]
    public List<Button> ItemButtons;

    private static InventoryScreen _current;

    public static InventoryScreen Current { get { return _current; } }

    private bool _isVisible;
    private int _currentIndex;

    private EquippedContext _equippedContext;

    // Mouse Swivel
    private Vehicle focusVehicle;
    private Vector2 _swivelVelocity;
    private Vector2 _swivel;

    // Double click
    private int _selectedItemIndex;
    private int _lastClickedIndex;
    private float _doubleClickCooldown;

    // Salvage bars
    private List<Image> _salvageBars;

    private void Awake()
    {
        _current = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (_isVisible)
        {
            if (Input.GetButtonUp("SquadronNext"))
                Next();

            if (Input.GetButtonUp("SquadronPrevious"))
                Previous();

            if (Input.GetKeyUp(KeyCode.Escape))
                Hide();
            PreviewMovement();
        }
        if (_doubleClickCooldown >=0)
        {
            _doubleClickCooldown -= Time.deltaTime;
        }
    }

    private void PreviewMovement()
    {
        if (Input.GetMouseButton(0))
        {
            _swivelVelocity = 180f * new Vector2(-Input.GetAxis("MouseHorizontal"), -Input.GetAxis("MouseVertical")) * Time.unscaledDeltaTime;
        }
        else
        {
            _swivelVelocity = Vector2.Lerp(_swivelVelocity, Vector2.zero, 5f * Time.unscaledDeltaTime);
        }
        _swivel.x -= _swivelVelocity.x;
        _swivel.y = Mathf.Clamp(_swivel.y - _swivelVelocity.y, -90f, 90f);
    }

    private void LateUpdate()
    {
        if (_isVisible)
        {
            if (focusVehicle != null)
            {
                Universe.Current.ViewPort.transform.position = focusVehicle.transform.position + Quaternion.AngleAxis(_swivel.x, focusVehicle.transform.up) * Quaternion.AngleAxis(_swivel.y, focusVehicle.transform.right) * (10f * focusVehicle.transform.forward + 5f * focusVehicle.transform.up + 5f * focusVehicle.transform.right);
                Universe.Current.ViewPort.transform.rotation = Quaternion.LookRotation(focusVehicle.transform.position - Universe.Current.ViewPort.transform.position, focusVehicle.transform.up);
            }
        }
    }

    public void Toggle()
    {
        if (_isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        _currentIndex = PlayerController.Current.Squadron.GetCurrentIndex();
        Populate(_currentIndex);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        HeadsUpDisplay.Current.gameObject.SetActive(false);
        TrackerManager.Current.SetTrackersVisibility(false);
        PlayerController.Current.SetControlEnabled(false);
        Universe.Current.ViewPort.SetFree(true);

        Canvas.enabled = true;
        _isVisible = true;
    }

    public void Hide()
    {
        Canvas.enabled = false;
        _isVisible = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HeadsUpDisplay.Current.gameObject.SetActive(true);
        TrackerManager.Current.SetTrackersVisibility(true);
        PlayerController.Current.SetControlEnabled(true);
        Universe.Current.ViewPort.SetFree(false);

        HeadsUpDisplay.Current.HideSquadronPrompt();
    }

    private void PopulateByContext()
    {
        if (_equippedContext == EquippedContext.Primary)
            PopulatePrimary();
        if (_equippedContext == EquippedContext.Secondary)
            PopulateSecondary();
        if (_equippedContext == EquippedContext.Shield)
            PopulateShield();
        if (_equippedContext == EquippedContext.Engine)
            PopulateEngine();
    }

    private void UpdateCredits()
    {
        CreditsText.text = string.Format("{0}c", PlayerController.Current.SpaceJunkCount);
        HeadsUpDisplay.Current.IncreaseSpaceJunk();
    }

    private void Populate(int squadronIndex)
    {
        var fighters = PlayerController.Current.Squadron.Members;
        var profile = fighters[squadronIndex].GetComponent<ShipProfile>();
        focusVehicle = fighters[squadronIndex].VehicleInstance;

        CallSignText.text = profile.CallSign;
        UpdateCredits();

        var items = PlayerController.Current.GetInventoryItems();

        _salvageBars = new List<Image>();

        for (var i = 0; i < ItemButtons.Count; i++)
        {
            ItemButtons[i].image.sprite = null;
            var index = i + 0;
            ItemButtons[i].onClick.AddListener(() => SelectItem(index));

            var inventButton = ItemButtons[i].GetComponent<InventoryButton>();
            if (ItemButtons[i].transform.childCount > 0)
            {
                var salvageBarTransform = inventButton.transform.GetChild(0);
                if (salvageBarTransform != null)
                {
                    var salvageBarImage = salvageBarTransform.GetComponentInChildren<Image>();
                    _salvageBars.Add(salvageBarImage);
                    inventButton.OnHoldAction = (holdFraction) => { UpdateSalvageItem(index, holdFraction); };
                    inventButton.OnHoldFinishAction = () => { SalvageItem(index); };
                    inventButton.OnReleaseAction = () => { SalvageCancel(index); };
                    inventButton.OnReleaseAction();
                }
            }
            if (items[i] != null)
            {
                ItemButtons[i].image.sprite = items[i].GetComponent<InventoryItem>().InventorySprite;
            }
        }

        if (focusVehicle != null)
        {
            KiaText.enabled = false;
            var primaryItem = focusVehicle.PrimaryWeaponInstance != null ? focusVehicle.PrimaryWeaponInstance.GetComponent<InventoryItem>() : null;
            var secondaryItem = focusVehicle.SecondaryWeaponInstance != null ? focusVehicle.SecondaryWeaponInstance.GetComponent<InventoryItem>() : null;
            var shieldItem = focusVehicle.ShieldInstance != null ? focusVehicle.ShieldInstance.GetComponent<InventoryItem>() : null;
            var engineItem = focusVehicle.EngineInstance != null ? focusVehicle.EngineInstance.GetComponent<InventoryItem>() : null;

            PrimaryButton.image.sprite = primaryItem != null ? primaryItem.InventorySprite : EmptyIcon;
            SecondaryButton.image.sprite = secondaryItem != null ? secondaryItem.InventorySprite : EmptyIcon;
            ShieldButton.image.sprite = shieldItem != null ? shieldItem.InventorySprite : EmptyIcon;
            EngineButton.image.sprite = engineItem != null ? engineItem.InventorySprite : EmptyIcon;

            PopulateByContext();
        }
        else
        {
            KiaText.enabled = true;

            PrimaryButton.image.sprite = EmptyIcon;
            SecondaryButton.image.sprite = EmptyIcon;
            ShieldButton.image.sprite = EmptyIcon;
            EngineButton.image.sprite = EmptyIcon;

            HideItemPanels();
        }
    }

    private void UpdateSalvageItem(int index, float fraction)
    {
        if (PlayerController.Current.GetInventoryItem(index) != null)
        {
            var itemButton = ItemButtons[index].transform.GetChild(0).GetComponentInChildren<Image>();
            if (itemButton != null)
                itemButton.fillAmount = fraction;
        }
    }

    private void SalvageItem(int index)
    {
        var salvageItem = PlayerController.Current.GetInventoryItem(index);
        if (salvageItem != null)
        {
            var salvageValue = salvageItem.GetComponent<InventoryItem>().SalvageValue;
            PlayerController.Current.SpaceJunkCount += salvageValue;
            PlayerController.Current.RemoveFromInventory(index);
            HeadsUpDisplay.Current.IncreaseSpaceJunk();
            PopulateByContext();
            UpdateCredits();
            ItemButtons[index].image.sprite = null;
            Debug.Log("SALVAGE ITEM: " + index + " FOR: " + salvageValue + "c");

            SalvageCancel(index);
        }
    }

    private void SalvageCancel(int index)
    {
        var itemButton = ItemButtons[index].transform.GetChild(0).GetComponentInChildren<Image>();
        if (itemButton != null)
            itemButton.fillAmount = 0f;
    }

    private void SelectItem(int index)
    {
        _selectedItemIndex = index;
        Debug.Log("SELECT ITEM " + _selectedItemIndex);
        if (index == _lastClickedIndex)
        {
            if (_doubleClickCooldown > 0f)
            {
                if (focusVehicle != null)
                {
                    if (PlayerController.Current.GetInventoryItem(index) != null)
                    {
                        var selectedInventoryItem = PlayerController.Current.GetInventoryItem(index).GetComponent<InventoryItem>();
                        Debug.Log("DOUBLE CLICK!");
                        Debug.Log("ITEM: " + selectedInventoryItem.name + " TYPE: " + selectedInventoryItem.Type);

                        // Primary Weapon
                        if (selectedInventoryItem.Type == ItemType.PrimaryWeapon)
                        {
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Weapon>().LootIndex;
                            if (focusVehicle.ShieldInstance != null)
                            {
                                var equippedItemIndex = focusVehicle.PrimaryWeaponInstance.LootIndex;
                                PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);
                                var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                                ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;
                            }

                            focusVehicle.Controller.GetComponent<ShipProfile>().PrimaryWeapon = LootManager.Current.Items[equipItemIndex].GetComponent<Weapon>();
                            focusVehicle.SetPrimaryWeapon(LootManager.Current.Items[equipItemIndex]);

                            PopulatePrimary();
                        }
                        // Secondary Weapon
                        if (selectedInventoryItem.Type == ItemType.SecondaryWeapon)
                        {
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Weapon>().LootIndex;
                            if (focusVehicle.ShieldInstance != null)
                            {
                                var equippedItemIndex = focusVehicle.SecondaryWeaponInstance.LootIndex;
                                PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);
                                var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                                ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;
                            }

                            focusVehicle.Controller.GetComponent<ShipProfile>().SecondaryWeapon = LootManager.Current.Items[equipItemIndex].GetComponent<Weapon>();
                            focusVehicle.SetSecondaryWeapon(LootManager.Current.Items[equipItemIndex]);

                            PopulateSecondary();
                        }
                        // Shield
                        if (selectedInventoryItem.Type == ItemType.Shield)
                        {
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Shield>().LootIndex;
                            if (focusVehicle.ShieldInstance != null)
                            {
                                var equippedItemIndex = focusVehicle.ShieldInstance.LootIndex;
                                PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);
                                var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                                ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;
                            }

                            focusVehicle.Controller.GetComponent<ShipProfile>().Shield = LootManager.Current.Items[equipItemIndex].GetComponent<Shield>();
                            focusVehicle.SetShield(LootManager.Current.Items[equipItemIndex]);

                            PopulateShield();
                        }
                        // Engine
                        if (selectedInventoryItem.Type == ItemType.Engine)
                        {
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Engine>().LootIndex;
                            if (focusVehicle.EngineInstance != null)
                            {
                                var equippedItemIndex = focusVehicle.EngineInstance.LootIndex;
                                PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);
                                var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                                ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;
                            }

                            focusVehicle.Controller.GetComponent<ShipProfile>().Engine = LootManager.Current.Items[equipItemIndex].GetComponent<Engine>();
                            focusVehicle.SetEngine(LootManager.Current.Items[equipItemIndex]);

                            PopulateEngine();
                        }
                    }
                }
            }
        }
        _doubleClickCooldown = 0.5f;
        _lastClickedIndex = index;
    }

    private void EnableEquippedButtons()
    {
        var isEnabled = true;
        PrimaryButton.enabled = isEnabled;
        SecondaryButton.enabled = isEnabled;
        ShieldButton.enabled = isEnabled;
        EngineButton.enabled = isEnabled;
    }

    private void HideItemPanels()
    {
        PrimaryPanel.gameObject.SetActive(false);
        SecondaryPanel.gameObject.SetActive(false);
        ShieldPanel.gameObject.SetActive(false);
        EnginePanel.gameObject.SetActive(false);
    }

    private float GetPointBarFraction(int points)
    {
        return Mathf.Clamp01(points / 10f);
    }

    private void PopulateWeaponPanel(int cost, int points, Text costText, Image pointsBar, Button addPointsButton)
    {
        if (points < 10)
        {
            costText.text = GetCostString(cost);
            costText.enabled = true;
            addPointsButton.gameObject.SetActive(PlayerController.Current.SpaceJunkCount >= cost);
        }
        else
        {
            costText.enabled = false;
            addPointsButton.gameObject.SetActive(false);
        }
        pointsBar.fillAmount = GetPointBarFraction(points);
    }

    public void PopulatePrimary()
    {
        var primaryWeapon = focusVehicle.PrimaryWeaponInstance;
        ItemNameText.text = primaryWeapon.Name;

        PopulateWeaponPanel(primaryWeapon.DamagePointCost, primaryWeapon.DamagePoints, PrimaryDamageCostText, PrimaryDamageBar, PrimaryAddDamageButton);
        PrimaryDamageValueText.text = string.Format("{0:f1}", primaryWeapon.Damage);

        PopulateWeaponPanel(primaryWeapon.FireRatePointCost, primaryWeapon.FireRatePoints, PrimaryFireRateCostText, PrimaryFireRateBar, PrimaryAddFireRateButton);
        PrimaryFireRateValueText.text = string.Format("{0:f1}/s", primaryWeapon.FireRate);

        PopulateWeaponPanel(primaryWeapon.CoolingRatePointCost, primaryWeapon.CoolingRatePoints, PrimaryCoolingRateCostText, PrimaryCoolingRateBar, PrimaryAddCoolingRateButton);
        PrimaryCoolingRateValueText.text = string.Format("{0:f1}/s", primaryWeapon.CoolingRate);

        PopulateWeaponPanel(primaryWeapon.HeatCapacityPointCost, primaryWeapon.HeatCapacityPoints, PrimaryHeatCapacityCostText, PrimaryHeatCapacityBar, PrimaryAddHeatCapacityButton);
        PrimaryHeatCapacityValueText.text = string.Format("{0:f1}", primaryWeapon.OverheatValue);

        var inventoryItem = primaryWeapon.GetComponent<InventoryItem>();
        if (inventoryItem != null)
            PrimaryButton.image.sprite = inventoryItem.InventorySprite;

        HideItemPanels();
        PrimaryPanel.gameObject.SetActive(true);

        _equippedContext = EquippedContext.Primary;
        EnableEquippedButtons();
        PrimaryButton.enabled = false;
    }

    private string GetCostString(int value)
    {
        if (PlayerController.Current.SpaceJunkCount >= value)
            return string.Format("<color=\"#0f0\">{0:f0}c</color>", value);
        return string.Format("<color=\"#f00\">{0:f0}c</color>", value);
    }

    public void AddDamagePoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Damage");
        if (_equippedContext == EquippedContext.Primary)
        {
            if (focusVehicle.PrimaryWeaponInstance.DamagePoints < 10  && PlayerController.Current.SpaceJunkCount > focusVehicle.PrimaryWeaponInstance.DamagePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.PrimaryWeaponInstance.DamagePointCost;
                focusVehicle.PrimaryWeaponInstance.DamagePoints++;
                PopulatePrimary();
                UpdateCredits();
            }
        }
        else
        {
            if (focusVehicle.SecondaryWeaponInstance.DamagePoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.SecondaryWeaponInstance.DamagePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.SecondaryWeaponInstance.DamagePointCost;
                focusVehicle.SecondaryWeaponInstance.DamagePoints++;
                PopulateSecondary();
                UpdateCredits();
            }
        }
    }

    public void AddFireRatePoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Fire Rate");
        if (_equippedContext == EquippedContext.Primary)
        {
            if (focusVehicle.PrimaryWeaponInstance.FireRatePoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.PrimaryWeaponInstance.FireRatePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.PrimaryWeaponInstance.FireRatePointCost;
                focusVehicle.PrimaryWeaponInstance.FireRatePoints++;
                PopulatePrimary();
                UpdateCredits();
            }
        }
        else
        {
            if (focusVehicle.SecondaryWeaponInstance.FireRatePoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.SecondaryWeaponInstance.FireRatePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.SecondaryWeaponInstance.FireRatePointCost;
                focusVehicle.SecondaryWeaponInstance.FireRatePoints++;
                PopulateSecondary();
                UpdateCredits();
            }
        }
    }

    public void AddCoolingRatePoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Cooling Rate");
        if (_equippedContext == EquippedContext.Primary)
        {
            if (focusVehicle.PrimaryWeaponInstance.CoolingRatePoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.PrimaryWeaponInstance.CoolingRatePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.PrimaryWeaponInstance.CoolingRatePointCost;
                focusVehicle.PrimaryWeaponInstance.CoolingRatePoints++;
                PopulatePrimary();
                UpdateCredits();
            }
        }
        else
        {
            if (focusVehicle.SecondaryWeaponInstance.CoolingRatePoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.SecondaryWeaponInstance.CoolingRatePointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.SecondaryWeaponInstance.CoolingRatePointCost;
                focusVehicle.SecondaryWeaponInstance.CoolingRatePoints++;
                PopulateSecondary();
                UpdateCredits();
            }
        }
    }

    public void AddHeatCapacityPoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Heat Capacity");
        if (_equippedContext == EquippedContext.Primary)
        {
            if (focusVehicle.PrimaryWeaponInstance.HeatCapacityPoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.PrimaryWeaponInstance.HeatCapacityPointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.PrimaryWeaponInstance.HeatCapacityPointCost;
                focusVehicle.PrimaryWeaponInstance.HeatCapacityPoints++;
                PopulatePrimary();
                UpdateCredits();
            }
        }
        else
        {
            if (focusVehicle.SecondaryWeaponInstance.HeatCapacityPoints < 10 && PlayerController.Current.SpaceJunkCount > focusVehicle.SecondaryWeaponInstance.HeatCapacityPointCost)
            {
                PlayerController.Current.SpaceJunkCount -= focusVehicle.SecondaryWeaponInstance.HeatCapacityPointCost;
                focusVehicle.SecondaryWeaponInstance.HeatCapacityPoints++;
                PopulateSecondary();
                UpdateCredits();
            }
        }
    }

    public void PopulateSecondary()
    {
        var secondaryWeapon = focusVehicle.SecondaryWeaponInstance;
        ItemNameText.text = secondaryWeapon.Name;

        PopulateWeaponPanel(secondaryWeapon.DamagePointCost, secondaryWeapon.DamagePoints, SecondaryDamageCostText, SecondaryDamageBar, SecondaryAddDamageButton);
        SecondaryDamageValueText.text = string.Format("{0:f1}", secondaryWeapon.Damage);

        PopulateWeaponPanel(secondaryWeapon.FireRatePointCost, secondaryWeapon.FireRatePoints, SecondaryFireRateCostText, SecondaryFireRateBar, SecondaryAddFireRateButton);
        SecondaryFireRateValueText.text = string.Format("{0:f1}/s", secondaryWeapon.FireRate);

        PopulateWeaponPanel(secondaryWeapon.CoolingRatePointCost, secondaryWeapon.CoolingRatePoints, SecondaryCoolingRateCostText, SecondaryCoolingRateBar, SecondaryAddCoolingRateButton);
        SecondaryCoolingRateValueText.text = string.Format("{0:f1}/s", secondaryWeapon.CoolingRate);

        PopulateWeaponPanel(secondaryWeapon.HeatCapacityPointCost, secondaryWeapon.HeatCapacityPoints, SecondaryHeatCapacityCostText, SecondaryHeatCapacityBar, SecondaryAddHeatCapacityButton);
        SecondaryHeatCapacityValueText.text = string.Format("{0:f1}", secondaryWeapon.OverheatValue);

        var inventoryItem = secondaryWeapon.GetComponent<InventoryItem>();
        if (inventoryItem != null)
            SecondaryButton.image.sprite = inventoryItem.InventorySprite;

        HideItemPanels();
        SecondaryPanel.gameObject.SetActive(true);

        _equippedContext = EquippedContext.Secondary;
        EnableEquippedButtons();
        SecondaryButton.enabled = false;
    }

    public void PopulateShield()
    {
        var shield = focusVehicle.ShieldInstance;
        HideItemPanels();

        if (shield != null)
        {
            ItemNameText.text = shield.Name;

            PopulateWeaponPanel(shield.CapacityPointCost, shield.CapacityPoints, ShieldCapacityCostText, ShieldCapacityBar, ShieldAddCapacityButton);
            ShieldCapacityValueText.text = string.Format("{0:f1}", shield.Capacity);

            PopulateWeaponPanel(shield.RegenerationPointCost, shield.RegenerationRatePoints, ShieldRegenerationCostText, ShieldRegenerationBar, ShieldAddRegenerationButton);
            ShieldRegenerationValueText.text = string.Format("{0:f1}/s", shield.RegenerationRate);

            var inventoryItem = shield.GetComponent<InventoryItem>();
            if (inventoryItem != null)
                ShieldButton.image.sprite = inventoryItem.InventorySprite;

            ShieldPanel.gameObject.SetActive(true);
        }
        else
        {
            ItemNameText.text = "No Shield Equipped";
        }

        _equippedContext = EquippedContext.Shield;
        EnableEquippedButtons();
        ShieldButton.enabled = false;
    }

    public void PopulateEngine()
    {
        var engine = focusVehicle.EngineInstance;
        HideItemPanels();

        if (engine != null)
        {
            ItemNameText.text = engine.Name;

            var inventoryItem = engine.GetComponent<InventoryItem>();
            if (inventoryItem != null)
                EngineButton.image.sprite = inventoryItem.InventorySprite;

            ShieldPanel.gameObject.SetActive(true);
        }
        else
        {
            ItemNameText.text = "No Engine Equipped.";
        }

        _equippedContext = EquippedContext.Engine;
        EnableEquippedButtons();
        EngineButton.enabled = false;
    }

    public void Next()
    {
        _currentIndex++;
        if (_currentIndex > PlayerController.Current.Squadron.Members.Count - 1)
            _currentIndex = 0;
        Populate(_currentIndex);
    }

    public void Previous()
    {
        _currentIndex--;
        if (_currentIndex < 0)
            _currentIndex = PlayerController.Current.Squadron.Members.Count - 1;
        Populate(_currentIndex);
    }

    public enum EquippedContext
    {
        Primary,
        Secondary,
        Shield,
        Engine
    }

    public enum PointsProperty
    {
        Damage,
        FireRate,
        CoolingRate,
        HeatCapacity
    }
}
