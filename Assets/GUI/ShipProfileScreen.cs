﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipProfileScreen : MonoBehaviour
{
    //public Vehicle VehiclePrefab;
    public List<Fighter> Fighters;

    public Canvas Canvas;
    public GameObject Preview;

    public Text CallSignValue;
    public Text ShipNameValue;
    public Text UnusedPowerNodes;
    //public Text PowerValue;

    public Image PowerValueContainer;
    public Image WeaponValueContainer;
    public Image ShieldValueContainer;
    public Image SpecialValueContainer;

    //public Text ShieldValue;
    public Text HullValue;

    public Text PrimaryWeaponText;
    public Text SecondaryWeaponText;

    private int _curIndex;
    private GameObject _preview;
    private static ShipProfileScreen _current;

    public static ShipProfileScreen Current { get { return _current; } }

    private bool _isVisible;

    private void Awake()
    {
        _current = this;
    }

    private void Start()
    {
        Hide(false);
        _curIndex = 0;
        if (Fighters.Count > 0)
            Populate(_curIndex);
    }

    private void Update()
    {
        if (_isVisible)
        {
            if (Input.GetButtonUp("SquadronNext"))
                ShowNext();

            if (Input.GetButtonUp("SquadronPrevious"))
                ShowPrevious();

            if (Input.GetKeyUp(KeyCode.Escape)) // || Input.GetKeyUp(KeyCode.H))
                Hide();

            /*
            if (_preview != null)
                _preview.transform.Rotate(Vector3.up, 15f*Time.unscaledDeltaTime);
            */

            if (_preview != null)
            {
                PreviewMovement();
            }
        }
    }

    private Vector2 swivelVelocity;
    private Vector2 swivel;

    private void PreviewMovement()
    {
        if (Input.GetMouseButton(0))
        {
            swivelVelocity = 180f * new Vector2(-Input.GetAxis("MouseHorizontal"), -Input.GetAxis("MouseVertical")) * Time.unscaledDeltaTime;
        }
        else
        {
            swivelVelocity = Vector2.Lerp(swivelVelocity, Vector2.zero, 5f*Time.unscaledDeltaTime);
        }
        swivel.x -= swivelVelocity.x;
        swivel.y = Mathf.Clamp(swivel.y - swivelVelocity.y, -90f, 90f);
        _preview.transform.rotation *= Quaternion.AngleAxis(swivelVelocity.x, Vector3.up);
    }

    public void ShowNext()
    {
        _curIndex++;
        if (_curIndex > Fighters.Count - 1)
            _curIndex = 0;
        Populate(_curIndex);
    }

    public void ShowPrevious()
    {
        _curIndex--;
        if (_curIndex < 0)
            _curIndex = Fighters.Count - 1;
        Populate(_curIndex);
    }

    private Vehicle focusVehicle;

    public void Populate(int index)
    {
        _curIndex = index;
        var fighter = Fighters[_curIndex];
        if (_preview != null)
            Destroy(_preview);
        _preview = (GameObject) Instantiate(fighter.VehiclePrefab.PreviewPrefab, new Vector3(0, 0.5f, 1f), Quaternion.Euler(25f, 200f, 10f));
        _preview.layer = LayerMask.NameToLayer("Preview");
        _preview.transform.SetParent(Preview.transform);

        var profile = fighter.GetComponent<ShipProfile>();
        CallSignValue.text = profile.CallSign;
        PopulatePowerBar(profile);
        PopulateBar(WeaponValueContainer, profile.Weapons);
        PopulateBar(ShieldValueContainer, profile.Shields);
        PopulateBar(SpecialValueContainer, profile.Special);

        ShipNameValue.text = fighter.VehiclePrefab.Name;
        //PowerValue.text = string.Format("{0:f0}", powerProfile.TotalPower);

        var vehicleKillable = fighter.VehiclePrefab.GetComponent<Killable>();
        //ShieldValue.text = string.Format("{0:f0}", vehicleKillable.MaxShield);
        HullValue.text = string.Format("{0:f0}/{1:f0}", vehicleKillable.Health, vehicleKillable.MaxHealth);

        PrimaryWeaponText.text = fighter.VehiclePrefab.PrimaryWeapon.Name;
        SecondaryWeaponText.text = fighter.VehiclePrefab.SecondaryWeapon.Name;

        Universe.Current.ViewPort.SetFree(true);
        focusVehicle = fighter.VehicleInstance;
    }

    private void LateUpdate()
    {
        if (_isVisible)
        {
            if (focusVehicle != null)
            {
                Universe.Current.ViewPort.transform.position = focusVehicle.transform.position + Quaternion.AngleAxis(swivel.x, focusVehicle.transform.up) * Quaternion.AngleAxis(swivel.y, focusVehicle.transform.right) * (10f * focusVehicle.transform.forward + 5f * focusVehicle.transform.up + 5f * focusVehicle.transform.right);
                Universe.Current.ViewPort.transform.rotation = Quaternion.LookRotation(focusVehicle.transform.position - Universe.Current.ViewPort.transform.position, focusVehicle.transform.up);
            }
        }
    }

    public void AddPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (Player.Current.PowerNodeCount > 0)
        {
            powerProfile.TotalPower++;
            Player.Current.PowerNodeCount--;
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void SubtractPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.TotalPower--;
            Player.Current.PowerNodeCount++;
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void AddWeaponPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Weapons++;
            PopulateBar(WeaponValueContainer, powerProfile.Weapons);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void SubtractWeaponPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.Weapons > 0)
        {
            powerProfile.Weapons--;
            PopulateBar(WeaponValueContainer, powerProfile.Weapons);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void AddShieldPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Shields++;
            PopulateBar(ShieldValueContainer, powerProfile.Shields);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void SubtractShieldPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.Shields > 0)
        {
            powerProfile.Shields--;
            PopulateBar(ShieldValueContainer, powerProfile.Shields);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void AddSpecialPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Special++;
            PopulateBar(SpecialValueContainer, powerProfile.Special);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    public void SubtractSpecialPower()
    {
        var member = Player.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<ShipProfile>();
        if (powerProfile.Special > 0)
        {
            powerProfile.Special--;
            PopulateBar(SpecialValueContainer, powerProfile.Special);
            PopulatePowerBar(powerProfile);
            ApplyPowerProfile(member, powerProfile);
        }
    }

    private void PopulatePowerBar(ShipProfile profile)
    {
        foreach (Transform t in PowerValueContainer.transform)
        {
            if (t != PowerValueContainer.transform)
                Destroy(t.gameObject);
        }
        for (var i = 0; i < 20; i++)
        {
            var counterObj = new GameObject(string.Format("counter_{0}", i));
            var counterImage = counterObj.AddComponent<Image>();
            var layoutElem = counterObj.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 5f;
            layoutElem.minHeight = 20f;

            if (i < profile.PowerRemaining)
            {
                counterImage.color = Color.white;
            }
            else
            {
                if (i < profile.TotalPower)
                {
                    counterImage.color = new Color(1f, 1f, 1f, 0.3f);
                }
                else {
                    counterImage.color = new Color(1f, 1f, 1f, 0.1f);
                }
            }

            counterObj.transform.SetParent(PowerValueContainer.transform);
            counterImage.rectTransform.localPosition = Vector3.zero;
            counterImage.rectTransform.localScale = Vector3.one;
            //counterImage.rectTransform.localRotation = Quaternion.identity;
        }
        if (Player.Current.PowerNodeCount > 0)
        {
            UnusedPowerNodes.text = string.Format("{0:f0}", Player.Current.PowerNodeCount);
            UnusedPowerNodes.enabled = true;
        }
        else
        {
            UnusedPowerNodes.enabled = false;
        }
    }

    private void PopulateBar(Image bar, int value)
    {
        foreach (Transform t in bar.transform)
        {
            if (t != bar.transform)
                Destroy(t.gameObject);
        }
        for (var i = 0; i < 20; i++)
        {
            var counterObj = new GameObject(string.Format("counter_{0}", i));
            var counterImage = counterObj.AddComponent<Image>();
            var layoutElem = counterObj.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 5f;
            layoutElem.minHeight = 20f;

            if (i < value)
            {
                counterImage.color = Color.white;
            }
            else
            {
                counterImage.color = new Color(1f, 1f, 1f, 0.1f);
            }

            counterObj.transform.SetParent(bar.transform);
            counterImage.rectTransform.localPosition = Vector3.zero;
            counterImage.rectTransform.localScale = Vector3.one;
            //counterImage.rectTransform.localRotation = Quaternion.identity;
        }
    }

    private void ApplyPowerProfile(Fighter member, ShipProfile profile)
    {
        member.VehicleInstance.Killable.MaxShield = profile.GetShield();
        //member.VehicleInstance.MaxBoostEnergy = profile.GetBoostEnergy();
        if (member.VehicleInstance.Killable.Shield > member.VehicleInstance.Killable.MaxShield)
            member.VehicleInstance.Killable.Shield = member.VehicleInstance.Killable.MaxShield;
        //if (member.VehicleInstance.BoostEnergy > member.VehicleInstance.MaxBoostEnergy)
        //    member.VehicleInstance.BoostEnergy = member.VehicleInstance.MaxBoostEnergy;
        member.VehicleInstance.TriggerBoostRegeneration();
    }

    public void Show()
    {
        HeadsUpDisplay.Current.gameObject.SetActive(false);
        TrackerManager.Current.SetTrackersVisibility(false);
        //Time.timeScale = 0f;
        Player.Current.SetControlEnabled(false);
        Canvas.gameObject.SetActive(true);
        Preview.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isVisible = true;
    }

    public void Hide(bool affectPlayerControl = true)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Preview.SetActive(false);
        Canvas.gameObject.SetActive(false);
        if (affectPlayerControl)
            Player.Current.SetControlEnabled(true);
        //Time.timeScale = 1f;
        HeadsUpDisplay.Current.gameObject.SetActive(true);
        TrackerManager.Current.SetTrackersVisibility(true);
        _isVisible = false;

        Universe.Current.ViewPort.SetFree(false);
    }
}
