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
        Hide();
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

            if (_preview != null)
                _preview.transform.Rotate(Vector3.up, 15f*Time.unscaledDeltaTime);
        }
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

    public void Populate(int index)
    {
        _curIndex = index;
        var fighter = Fighters[_curIndex];
        if (_preview != null)
            Destroy(_preview);
        _preview = (GameObject) Instantiate(fighter.VehiclePrefab.PreviewPrefab, new Vector3(0, 0.5f, 1f), Quaternion.Euler(25f, 200f, 10f));
        _preview.layer = LayerMask.NameToLayer("Preview");
        _preview.transform.SetParent(Preview.transform);
        CallSignValue.text = fighter.CallSign;

        var powerProfile = fighter.GetComponent<PowerProfile>();

        PopulatePowerBar(powerProfile);
        PopulateBar(WeaponValueContainer, powerProfile.Weapons);
        PopulateBar(ShieldValueContainer, powerProfile.Shields);
        PopulateBar(SpecialValueContainer, powerProfile.Special);

        ShipNameValue.text = fighter.VehiclePrefab.Name;
        //PowerValue.text = string.Format("{0:f0}", powerProfile.TotalPower);

        var vehicleKillable = fighter.VehiclePrefab.GetComponent<Killable>();
        //ShieldValue.text = string.Format("{0:f0}", vehicleKillable.MaxShield);
        HullValue.text = string.Format("{0:f0}", vehicleKillable.MaxHealth);

        PrimaryWeaponText.text = fighter.VehiclePrefab.PrimaryWeaponPrefab.Name;
        SecondaryWeaponText.text = fighter.VehiclePrefab.SecondaryWeaponPrefab.Name;
    }

    public void AddWeaponPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Weapons++;
            PopulateBar(WeaponValueContainer, powerProfile.Weapons);
            PopulatePowerBar(powerProfile);
        }
    }

    public void SubtractWeaponPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.Weapons > 0)
        {
            powerProfile.Weapons--;
            PopulateBar(WeaponValueContainer, powerProfile.Weapons);
            PopulatePowerBar(powerProfile);
        }
    }

    public void AddShieldPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Shields++;
            PopulateBar(ShieldValueContainer, powerProfile.Shields);
            PopulatePowerBar(powerProfile);
        }
    }

    public void SubtractShieldPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.Shields > 0)
        {
            powerProfile.Shields--;
            PopulateBar(ShieldValueContainer, powerProfile.Shields);
            PopulatePowerBar(powerProfile);
        }
    }

    public void AddSpecialPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.PowerRemaining > 0)
        {
            powerProfile.Special++;
            PopulateBar(SpecialValueContainer, powerProfile.Special);
            PopulatePowerBar(powerProfile);
        }
    }

    public void SubtractSpecialPower()
    {
        var member = PlayerController.Current.Squadron.Members[_curIndex];
        var powerProfile = member.GetComponent<PowerProfile>();
        if (powerProfile.Special > 0)
        {
            powerProfile.Special--;
            PopulateBar(SpecialValueContainer, powerProfile.Special);
            PopulatePowerBar(powerProfile);
        }
    }

    private void PopulatePowerBar(PowerProfile profile)
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

    public void Show()
    {
        HeadsUpDisplay.Current.gameObject.SetActive(false);
        TrackerManager.Current.SetTrackersVisibility(false);
        Time.timeScale = 0f;
        PlayerController.Current.SetControlEnabled(false);
        Canvas.gameObject.SetActive(true);
        Preview.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isVisible = true;
    }

    public void Hide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Preview.SetActive(false);
        Canvas.gameObject.SetActive(false);
        PlayerController.Current.SetControlEnabled(true);
        Time.timeScale = 1f;
        HeadsUpDisplay.Current.gameObject.SetActive(true);
        TrackerManager.Current.SetTrackersVisibility(true);
        _isVisible = false;
    }
}
