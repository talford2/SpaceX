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

    [Header("Item Panel")]
    public Text ItemNameText;

    [Header("Primary Panel")]
    public CanvasGroup PrimaryPanel;
    public Text PrimaryDamageCostText;
    public Image PrimaryDamageBar;
    public Text PrimaryDamageValueText;

    public Text PrimaryFireRateCostText;
    public Image PrimaryFireRateBar;
    public Text PrimaryFireRateValueText;

    public Text PrimaryCoolingRateCostText;
    public Image PrimaryCoolingRateBar;
    public Text PrimaryCoolingRateValueText;

    public Text PrimaryHeatCapacityCostText;
    public Image PrimaryHeatCapacityBar;
    public Text PrimaryHeatCapacityValueText;

    [Header("Secondary Panel")]
    public CanvasGroup SecondaryPanel;

    public Text SecondaryDamageCostText;
    public Image SecondaryDamageBar;
    public Text SecondaryDamageValueText;

    public Text SecondaryFireRateCostText;
    public Image SecondaryFireRateBar;
    public Text SecondaryFireRateValueText;

    public Text SecondaryCoolingRateCostText;
    public Image SecondaryCoolingRateBar;
    public Text SecondaryCoolingRateValueText;

    public Text SecondaryHeatCapacityCostText;
    public Image SecondaryHeatCapacityBar;
    public Text SecondaryHeatCapacityValueText;

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

    private void UpdateCredits()
    {
        CreditsText.text = string.Format("{0}c", PlayerController.Current.SpaceJunkCount);
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
            var primaryItem = focusVehicle.PrimaryWeaponInstance.GetComponent<InventoryItem>();
            var secondaryItem = focusVehicle.SecondaryWeaponInstance.GetComponent<InventoryItem>();

            PrimaryButton.image.sprite = primaryItem != null ? primaryItem.InventorySprite : null;
            SecondaryButton.image.sprite = secondaryItem != null ? secondaryItem.InventorySprite : null;

            if (_equippedContext == EquippedContext.Primary)
                PopulatePrimary();
            if (_equippedContext == EquippedContext.Secondary)
                PopulateSecondary();
            if (_equippedContext == EquippedContext.Shield)
                PopulateShield();
            if (_equippedContext == EquippedContext.Engine)
                PopulateEngine();
        }
        else
        {
            KiaText.enabled = true;

            PrimaryButton.image.sprite = null;
            SecondaryButton.image.sprite = null;

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
                        var selectedInvenotyItem = PlayerController.Current.GetInventoryItem(index).GetComponent<InventoryItem>();
                        Debug.Log("DOUBLE CLICK!");
                        Debug.Log("ITEM: " + selectedInvenotyItem.name + " TYPE: " + selectedInvenotyItem.Type);

                        // Primary Weapon
                        if (selectedInvenotyItem.Type == ItemType.PrimaryWeapon)
                        {
                            var equippedItemIndex = focusVehicle.PrimaryWeaponInstance.LootIndex;
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Weapon>().LootIndex;

                            PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);

                            focusVehicle.Controller.GetComponent<ShipProfile>().PrimaryWeapon = LootManager.Current.Items[equipItemIndex].GetComponent<Weapon>();
                            focusVehicle.SetPrimaryWeapon(LootManager.Current.Items[equipItemIndex]);

                            var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                            ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;

                            PopulatePrimary();
                        }
                        // Secondary Weapon
                        if (selectedInvenotyItem.Type == ItemType.SecondaryWeapon)
                        {
                            var equippedItemIndex = focusVehicle.SecondaryWeaponInstance.LootIndex;
                            var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Weapon>().LootIndex;

                            PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);

                            focusVehicle.Controller.GetComponent<ShipProfile>().SecondaryWeapon = LootManager.Current.Items[equipItemIndex].GetComponent<Weapon>();
                            focusVehicle.SetSecondaryWeapon(LootManager.Current.Items[equipItemIndex]);

                            var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                            ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;

                            PopulateSecondary();
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
    }

    public void PopulatePrimary()
    {
        var primaryWeapon = focusVehicle.PrimaryWeaponInstance;
        ItemNameText.text = primaryWeapon.Name;
        PrimaryDamageCostText.text = GetCostString(100f);
        PrimaryDamageValueText.text = string.Format("{0:f1}", primaryWeapon.MissileDamage);

        PrimaryFireRateCostText.text = GetCostString(100f);
        PrimaryFireRateValueText.text = string.Format("{0:f1}/s", 1f / primaryWeapon.FireRate);

        PrimaryCoolingRateCostText.text = GetCostString(100f);
        PrimaryCoolingRateValueText.text = string.Format("{0:f1}/s", primaryWeapon.CoolingRate);

        PrimaryHeatCapacityCostText.text = GetCostString(100f);
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

    private string GetCostString(float value)
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
            PrimaryDamageBar.fillAmount = Mathf.Clamp01(PrimaryDamageBar.fillAmount + 0.1f);
        }
        else
        {
            SecondaryDamageBar.fillAmount = Mathf.Clamp01(SecondaryDamageBar.fillAmount + 0.1f);
        }
    }

    public void AddFireRatePoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Fire Rate");
        if (_equippedContext == EquippedContext.Primary)
        {
            PrimaryFireRateBar.fillAmount = Mathf.Clamp01(PrimaryFireRateBar.fillAmount + 0.1f);
        }
        else
        {
            SecondaryFireRateBar.fillAmount = Mathf.Clamp01(SecondaryFireRateBar.fillAmount + 0.1f);
        }
    }

    public void AddCoolingRatePoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Cooling Rate");
        if (_equippedContext == EquippedContext.Primary)
        {
            PrimaryCoolingRateBar.fillAmount = Mathf.Clamp01(PrimaryCoolingRateBar.fillAmount + 0.1f);
        }
        else
        {
            SecondaryCoolingRateBar.fillAmount = Mathf.Clamp01(SecondaryCoolingRateBar.fillAmount + 0.1f);
        }
    }

    public void AddHeatCapacityPoints()
    {
        Debug.Log("ADD: " + _equippedContext + ", Heat Capacity");
        if (_equippedContext == EquippedContext.Primary)
        {
            PrimaryHeatCapacityBar.fillAmount = Mathf.Clamp01(PrimaryHeatCapacityBar.fillAmount + 0.1f);
        }
        else
        {
            SecondaryHeatCapacityBar.fillAmount = Mathf.Clamp01(SecondaryHeatCapacityBar.fillAmount + 0.1f);
        }
    }

    public void PopulateSecondary()
    {
        var secondaryWeapon = focusVehicle.SecondaryWeaponInstance;
        ItemNameText.text = secondaryWeapon.Name;
        SecondaryDamageCostText.text = GetCostString(100f);
        SecondaryDamageValueText.text = string.Format("{0:f1}", secondaryWeapon.MissileDamage);

        SecondaryCoolingRateCostText.text = GetCostString(100f);
        SecondaryFireRateValueText.text = string.Format("{0:f1}/s", 1f / secondaryWeapon.FireRate);

        SecondaryCoolingRateCostText.text = GetCostString(100f);
        SecondaryCoolingRateValueText.text = string.Format("{0:f1}/s", secondaryWeapon.CoolingRate);

        SecondaryHeatCapacityCostText.text = GetCostString(100f);
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
        ItemNameText.text = "Shield";

        HideItemPanels();

        _equippedContext = EquippedContext.Shield;
        EnableEquippedButtons();
        ShieldButton.enabled = false;
    }

    public void PopulateEngine()
    {
        ItemNameText.text = "Engine";

        HideItemPanels();

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
