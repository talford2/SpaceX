using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScreen : MonoBehaviour
{
    public Canvas Canvas;
    public Text CallSignText;
    public Text CreditsText;

    [Header("Equipped Panel")]
    public Button PrimaryButton;
    public Button SecondaryButton;
    public Button ShieldButton;
    public Button EngineButton;

    [Header("Item Panel")]
    public Text ItemNameText;

    [Header("Primary Panel")]
    public CanvasGroup PrimaryPanel;
    public Text PrimaryDamageValueText;
    public Text PrimaryFireRateValueText;
    public Text PrimaryCoolingRateValueText;
    public Text PrimaryHeatCapacityValueText;

    [Header("Secondary Panel")]
    public CanvasGroup SecondaryPanel;
    public Text SecondaryDamageValueText;
    public Text SecondaryFireRateValueText;
    public Text SecondaryCoolingRateValueText;
    public Text SecondaryHeatCapacityValueText;

    [Header("Inventory Panel")]
    public List<Button> ItemButtons;

    private static InventoryScreen _current;

    public static InventoryScreen Current { get { return _current; } }

    private bool _isVisible;
    private int _currentIndex;

    private Equippedcontext _equippedContext;

    // Mouse Swivel
    private Vehicle focusVehicle;
    private Vector2 _swivelVelocity;
    private Vector2 _swivel;

    // Double click
    private int _selectedItemIndex;
    private int _lastClickedIndex;
    private float _doubleClickCooldown;

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
        PopulatePrimary();

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

    private void Populate(int squadronIndex)
    {
        var fighters = PlayerController.Current.Squadron.Members;
        var profile = fighters[squadronIndex].GetComponent<ShipProfile>();
        focusVehicle = fighters[squadronIndex].VehicleInstance;

        CallSignText.text = profile.CallSign;
        CreditsText.text = string.Format("{0}c", PlayerController.Current.SpaceJunkCount);

        var items = PlayerController.Current.GetInventoryItems();
        for (var i = 0; i < ItemButtons.Count; i++)
        {
            ItemButtons[i].image.sprite = null;
            var index = i + 0;
            ItemButtons[i].onClick.AddListener(() => SelectItem(index));
            if (items[i] != null)
            {
                ItemButtons[i].image.sprite = items[i].GetComponent<InventoryItem>().InventorySprite;
            }
        }

        var primaryItem = focusVehicle.PrimaryWeaponPrefab.GetComponent<InventoryItem>();
        var secondaryItem = focusVehicle.SecondaryWeaponPrefab.GetComponent<InventoryItem>();

        PrimaryButton.image.sprite = primaryItem != null ? primaryItem.InventorySprite : null;
        SecondaryButton.image.sprite = secondaryItem != null ? secondaryItem.InventorySprite : null;

        if (_equippedContext == Equippedcontext.Primary)
            PopulatePrimary();
        if (_equippedContext == Equippedcontext.Secondary)
            PopulateSecondary();
        if (_equippedContext == Equippedcontext.Shield)
            PopulateShield();
        if (_equippedContext == Equippedcontext.Engine)
            PopulateEngine();
    }

    private void SelectItem(int index)
    {
        //ItemButtons[_selectedItemIndex].enabled = true;
        _selectedItemIndex = index;
        //ItemButtons[index].enabled = false;
        Debug.Log("SELECT ITEM " + _selectedItemIndex);
        if (index == _lastClickedIndex)
        {
            if (_doubleClickCooldown > 0f)
            {
                Debug.Log("DOUBLE CLICK!");
                var equippedItemIndex = focusVehicle.PrimaryWeaponInstance.LootIndex;
                var equipItemIndex = PlayerController.Current.GetInventoryItem(index).GetComponent<Weapon>().LootIndex;

                PlayerController.Current.SetInventoryItem(index, LootManager.Current.Items[equippedItemIndex]);

                focusVehicle.SetPrimaryWeapon(LootManager.Current.Items[equipItemIndex]);

                var inventoryItem = LootManager.Current.Items[equippedItemIndex].GetComponent<InventoryItem>();
                ItemButtons[index].image.sprite = inventoryItem != null ? inventoryItem.InventorySprite : null;

                PopulatePrimary();
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
        PrimaryDamageValueText.text = string.Format("{0:f1}", primaryWeapon.MissileDamage);
        PrimaryFireRateValueText.text = string.Format("{0:f1}", primaryWeapon.FireRate);

        var inventoryItem = primaryWeapon.GetComponent<InventoryItem>();
        if (inventoryItem != null)
            PrimaryButton.image.sprite = inventoryItem.InventorySprite;

        HideItemPanels();
        PrimaryPanel.gameObject.SetActive(true);

        _equippedContext = Equippedcontext.Primary;
        EnableEquippedButtons();
        PrimaryButton.enabled = false;
    }

    public void PopulateSecondary()
    {
        var secondaryWeapon = focusVehicle.SecondaryWeaponInstance;
        ItemNameText.text = secondaryWeapon.Name;
        SecondaryDamageValueText.text = string.Format("{0:f1}", secondaryWeapon.MissileDamage);
        SecondaryFireRateValueText.text = string.Format("{0:f1}", secondaryWeapon.FireRate);

        HideItemPanels();
        SecondaryPanel.gameObject.SetActive(true);

        _equippedContext = Equippedcontext.Secondary;
        EnableEquippedButtons();
        SecondaryButton.enabled = false;
    }

    public void PopulateShield()
    {
        ItemNameText.text = "Shield";

        HideItemPanels();

        _equippedContext = Equippedcontext.Shield;
        EnableEquippedButtons();
        ShieldButton.enabled = false;
    }

    public void PopulateEngine()
    {
        ItemNameText.text = "Engine";

        HideItemPanels();

        _equippedContext = Equippedcontext.Engine;
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

    private enum Equippedcontext
    {
        Primary,
        Secondary,
        Shield,
        Engine
    }
}
