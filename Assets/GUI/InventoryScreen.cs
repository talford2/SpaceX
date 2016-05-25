﻿using UnityEngine;
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

    private static InventoryScreen _current;

    public static InventoryScreen Current { get { return _current; } }

    private bool _isVisible;
    private int _currentIndex;

    // Mouse Swivel
    private Vehicle focusVehicle;
    private Vector2 _swivelVelocity;
    private Vector2 _swivel;

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
    }

    private void EnableEquippedButtons()
    {
        var isEnabled = true;
        PrimaryButton.enabled = isEnabled;
        SecondaryButton.enabled = isEnabled;
        ShieldButton.enabled = isEnabled;
        EngineButton.enabled = isEnabled;
    }

    public void PopulatePrimary()
    {
        var primaryWeapon = focusVehicle.PrimaryWeaponInstance;
        ItemNameText.text = primaryWeapon.Name;

        EnableEquippedButtons();
        PrimaryButton.enabled = false;
    }

    public void PopulateSecondary()
    {
        var secondaryWeapon = focusVehicle.SecondaryWeaponInstance;
        ItemNameText.text = secondaryWeapon.Name;

        EnableEquippedButtons();
        SecondaryButton.enabled = false;
    }

    public void PopulateShield()
    {
        ItemNameText.text = "Shield";

        EnableEquippedButtons();
        ShieldButton.enabled = false;
    }

    public void PopulateEngine()
    {
        ItemNameText.text = "Engine";

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
}
